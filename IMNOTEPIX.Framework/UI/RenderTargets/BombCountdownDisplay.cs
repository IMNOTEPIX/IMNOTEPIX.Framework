using GTA;
using GTA.Native;

namespace IMNOTEPIX.Framework.fUI.RenderTargets
{
    /// <summary>
    /// Bomb-countdown-only slice of the decompiled rendertarget/scaleform
    /// system (func_19506 state machine + func_1148's h4_ld_bomb_02a branch).
    /// Same honesty notes as before still apply:
    /// - The state machine is reconstructed as persistent per-instance state;
    ///   the decompile itself shows the switch variable as a fresh local
    ///   reset to 0 every call, which can't be the real runtime behavior.
    /// - The countdown value passed to SET_TIME (milliseconds, per the
    ///   confirmed native signature) is tracked here via real elapsed frame
    ///   time (Game.LastFrameTime), but only decremented once
    ///   TriggerCountdown() has been called - the original excerpt didn't
    ///   show where the source value (iLocal_20367[uParam0->f_15]) came from
    ///   or how/when it started counting, so both the elapsed-time approach
    ///   and the explicit trigger gate are my construction, not a direct port.
    /// - FlashingTimeThresholdMs calls the scaleform's own confirmed
    ///   SET_FLASHING_TIME_THRESHOLD(milliseconds) method (found directly in
    ///   BOMB_COUNTDOWN's method list) rather than faking a blink client-side.
    ///   Whether it needs to be sent once or every tick isn't confirmable
    ///   from the method list alone - sending once on first Ready tick is my
    ///   assumption.
    /// - Native names are taken verbatim from the decompile; still worth
    ///   double-checking DRAW_SPRITE_NAMED_RENDERTARGET / SET_TEXT_RENDER_ID
    ///   against docs.fivem.net/natives since I haven't independently
    ///   verified the SHVDN Hash enum spelling for those two.
    /// </summary>
    public class BombCountdownDisplay
    {
        private enum Step { Init, RequestAssets, RegisterTarget, LinkTarget, Ready }

        private Step _step = Step.Init;

        private readonly int _modelHash = Function.Call<int>(Hash.GET_HASH_KEY, "h4_prop_h4_ld_bomb_02a");
        private const string RenderTargetName = "Prop_h4_LD_Bomb_02a";
        private const string ScaleformName = "BOMB_COUNTDOWN";
        private const string SetTimeMethodName = "SET_TIME";

        private int _renderTargetRenderId = -1;
        private int _scaleformHandle;

        // f_15 - quadrant index (0-3), also selects draw position below.
        // Set this before the first Update() call; -1 defaults to 0.
        public int QuadrantIndex = -1;

        // f_10..f_13
        private float _drawX, _drawY;
        private const float DrawWidth = 0.9f;
        private const float DrawHeight = 0.9f;

        // Countdown state for SET_TIME(displayIndex, milliseconds).
        // SetCountdownMilliseconds only sets the starting value now - it does
        // NOT start ticking. Call TriggerCountdown() separately when you
        // actually want it to start counting down (e.g. bomb armed). Until
        // triggered, the display still shows the configured starting value
        // (SET_TIME is still sent every tick so the display initializes
        // correctly), it just won't decrement.
        private int _totalMilliseconds = -1;
        private float _remainingMilliseconds = -1f;
        private bool _isTicking = false;

        /// <summary>
        /// How many countdown-milliseconds pass per real millisecond.
        /// 1x (default) = real-time, 1 second removed per real second.
        /// Set to 60x for 1 in-game minute removed per real second.
        /// </summary>
        public float SpeedMultiplier = 1f;

        public bool IsTicking => _isTicking;

        /// <summary>Sets the countdown's starting value. Does not start it ticking.</summary>
        public void SetCountdownMilliseconds(int milliseconds)
        {
            _totalMilliseconds = milliseconds;
            _remainingMilliseconds = milliseconds;
            _isTicking = false;
        }

        /// <summary>Starts the countdown ticking down from its current value.</summary>
        public void TriggerCountdown()
        {
            _isTicking = true;
        }

        /// <summary>Pauses the countdown without resetting its remaining value.</summary>
        public void PauseCountdown()
        {
            _isTicking = false;
        }

        // --- Low-time flash ---
        // Confirmed via BOMB_COUNTDOWN's own method list: SET_FLASHING_TIME_THRESHOLD(milliseconds)
        // is a real scaleform method - the movie handles blinking internally,
        // so there's no need to fake it by skipping draw calls client-side
        // (that was my earlier workaround before I knew this existed).
        // Assumption I can't confirm from the method list alone: that this
        // only needs to be sent once and the scaleform remembers it, rather
        // than needing to be resent every tick. Sending it once on the first
        // Ready tick; move the call into UpdateSetTime if it turns out the
        // scaleform doesn't persist it.
        private bool _flashThresholdSent = false;

        /// <summary>
        /// Milliseconds remaining at which the scaleform should start
        /// blinking the display. Null (default) means never call it - the
        /// scaleform's own default behavior (if any) applies instead.
        /// </summary>
        public int? FlashingTimeThresholdMs = null;

        private void SendFlashingTimeThresholdIfNeeded()
        {
            if (_flashThresholdSent || FlashingTimeThresholdMs == null)
                return;

            Function.Call(Hash.BEGIN_SCALEFORM_MOVIE_METHOD, _scaleformHandle, "SET_FLASHING_TIME_THRESHOLD");
            Function.Call(Hash.SCALEFORM_MOVIE_METHOD_ADD_PARAM_INT, FlashingTimeThresholdMs.Value);
            Function.Call(Hash.END_SCALEFORM_MOVIE_METHOD);

            _flashThresholdSent = true;
        }

        // Entity the prop is attached to, for the on-screen check (func_19508
        // resolves this via func_352, which wasn't in the excerpt - wire up
        // however you track the prop entity in your framework).
        public GTA.Entity PropEntity;

        public bool IsReady => _step == Step.Ready && _renderTargetRenderId > -1;

        public void Update()
        {
            switch (_step)
            {
                case Step.Init:
                    ResolveQuadrant();
                    _step = Step.RequestAssets;
                    break;

                case Step.RequestAssets:
                    if (RequestAssets())
                        _step = Step.RegisterTarget;
                    break;

                case Step.RegisterTarget:
                    if (RegisterTarget())
                        _step = Step.LinkTarget;
                    break;

                case Step.LinkTarget:
                    if (LinkTarget())
                        _step = Step.Ready;
                    break;

                case Step.Ready:
                    SendFlashingTimeThresholdIfNeeded();
                    UpdateSetTime();
                    if (ShouldDraw())
                        Draw();
                    break;
            }
        }

        // func_1148's bomb-prop branch, quadrant part only
        private void ResolveQuadrant()
        {
            int index = QuadrantIndex > -1 ? QuadrantIndex : 0;
            QuadrantIndex = index;

            switch (index)
            {
                case 0: _drawX = 0.42f; _drawY = 0.4f; break;
                case 1: _drawX = -0.035f; _drawY = 0.4f; break;
                case 2: _drawX = 0.42f; _drawY = -0.06f; break;
                case 3: _drawX = -0.035f; _drawY = -0.06f; break;
                default: _drawX = 0.42f; _drawY = 0.4f; break;
            }
        }

        // func_19530
        private bool RequestAssets()
        {
            _scaleformHandle = Function.Call<int>(Hash.REQUEST_SCALEFORM_MOVIE, ScaleformName);
            return Function.Call<bool>(Hash.HAS_SCALEFORM_MOVIE_LOADED, _scaleformHandle);
        }

        // func_19529
        private bool RegisterTarget()
        {
            if (Function.Call<bool>(Hash.IS_NAMED_RENDERTARGET_REGISTERED, RenderTargetName))
                return true;

            return Function.Call<bool>(Hash.REGISTER_NAMED_RENDERTARGET, RenderTargetName, false);
        }

        // func_19528 - the two-tick link/get-render-id sequence. Do not
        // collapse IS_NAMED_RENDERTARGET_LINKED and LINK_NAMED_RENDERTARGET
        // into the same tick; the original checks the link state again on a
        // later call, only fetching the render ID once it reports linked.
        private bool LinkTarget()
        {
            if (_renderTargetRenderId > -1)
                return true;

            if (!Function.Call<bool>(Hash.IS_NAMED_RENDERTARGET_LINKED, _modelHash))
            {
                Function.Call(Hash.LINK_NAMED_RENDERTARGET, _modelHash);
                return false;
            }

            _renderTargetRenderId = Function.Call<int>(Hash.GET_NAMED_RENDERTARGET_RENDER_ID, RenderTargetName);
            return true;
        }

        // func_19510 - SET_TIME call. Original guards this with flag-bit
        // checks I couldn't confirm the meaning of; gating on IsReady here
        // instead, which is equivalent for this single prop case.
        private void UpdateSetTime()
        {
            if (_remainingMilliseconds < 0f)
                return; // caller hasn't set a starting value yet

            // Only actually count down once TriggerCountdown() has been
            // called. Still resend SET_TIME every tick regardless, so the
            // display shows the configured value even before triggering.
            if (_isTicking)
            {
                _remainingMilliseconds -= Game.LastFrameTime * 1000f * SpeedMultiplier;
                if (_remainingMilliseconds < 0f)
                    _remainingMilliseconds = 0f;
            }

            // SET_TIME(displayIndex, milliseconds) - confirmed by your screenshot.
            Function.Call(Hash.BEGIN_SCALEFORM_MOVIE_METHOD, _scaleformHandle, SetTimeMethodName);
            Function.Call(Hash.SCALEFORM_MOVIE_METHOD_ADD_PARAM_INT, QuadrantIndex);           // displayIndex
            Function.Call(Hash.SCALEFORM_MOVIE_METHOD_ADD_PARAM_INT, (int)_remainingMilliseconds); // milliseconds
            Function.Call(Hash.END_SCALEFORM_MOVIE_METHOD);
        }

        /// <summary>
        /// Releases the named rendertarget and marks the scaleform as no
        /// longer needed. Call when you're done with this prop instance
        /// (e.g. mission cleanup, entity deleted, script abort).
        /// SET_SCALEFORM_MOVIE_AS_NO_LONGER_NEEDED takes the handle by
        /// reference and zeroes it out - this is a well-established native
        /// (confirmed via community/ESX usage), not something present in
        /// your original decompiled excerpt.
        /// </summary>
        public void Dispose()
        {
            if (_renderTargetRenderId > -1)
            {
                Function.Call(Hash.RELEASE_NAMED_RENDERTARGET, RenderTargetName);
                _renderTargetRenderId = -1;
            }

            if (_scaleformHandle != 0)
            {
                int handle = _scaleformHandle;
                unsafe { Function.Call(Hash.SET_SCALEFORM_MOVIE_AS_NO_LONGER_NEEDED, &handle); }
                _scaleformHandle = 0;
            }

            _totalMilliseconds = -1;
            _remainingMilliseconds = -1f;
            _isTicking = false;
            _flashThresholdSent = false;
            _step = Step.Init;
        }

        // func_19508
        private bool ShouldDraw()
        {
            if (PropEntity == null || !PropEntity.Exists())
                return false;

            return PropEntity.IsOnScreen;
        }

        // func_19507
        private void Draw()
        {
            Function.Call(Hash.SET_TEXT_RENDER_ID, _renderTargetRenderId);
            Function.Call(Hash.SET_SCRIPT_GFX_ALIGN, 73, 73);
            Function.Call(Hash.SET_SCRIPT_GFX_DRAW_ORDER, 4);
            Function.Call(Hash.SET_SCRIPT_GFX_DRAW_BEHIND_PAUSEMENU, true);

            Function.Call(Hash.DRAW_SCALEFORM_MOVIE, _scaleformHandle, _drawX, _drawY, DrawWidth, DrawHeight, 255, 255, 255, 255, 0);

            Function.Call(Hash.RESET_SCRIPT_GFX_ALIGN);
            Function.Call(Hash.SET_TEXT_RENDER_ID, Function.Call<int>(Hash.GET_DEFAULT_SCRIPT_RENDERTARGET_RENDER_ID));
        }
    }
}