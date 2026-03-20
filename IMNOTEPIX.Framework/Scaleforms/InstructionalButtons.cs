using GTA;
using GTA.Native;
using System;
using System.Collections.Generic;

namespace IMNOTEPIX.Framework.Scaleforms
{
    /// <summary>
    /// A single instructional button displayed at the bottom of the screen.
    /// </summary>
    public class InstructionalButton
    {
        public Control Control { get; set; }
        public string Text { get; set; }

        public InstructionalButton(Control control, string text)
        {
            Control = control;
            Text = text;
        }

        /// <summary>
        /// Returns the button ID string for the scaleform.
        /// Automatically resolves to keyboard or controller glyph based on the
        /// current input method.
        /// </summary>
        public string GetButtonId()
            => Function.Call<string>(
                Hash.GET_CONTROL_INSTRUCTIONAL_BUTTONS_STRING,
                2, (int)Control, true);
    }

    /// <summary>
    /// Manages and renders a row of instructional buttons at the bottom of the screen
    /// using the instructional_buttons scaleform movie.
    /// </summary>
    public class InstructionalButtons : IDisposable
    {
        // 0xA571D46727E2B718 = _GET_LAST_INPUT_METHOD
        // Returns 0 for controller, 1 for keyboard/mouse.
        // No named Hash enum entry exists in SHVDN for this native.
        private static readonly Hash GetLastInputMethodHash = (Hash)0xA571D46727E2B718;

        private readonly Scaleform _scaleform;
        private readonly List<InstructionalButton> _buttons;

        private bool _needsUpdate;
        private int _lastInputMethod;

        /// <summary>Read-only view of the current button list.</summary>
        public IReadOnlyList<InstructionalButton> Buttons => _buttons;

        public InstructionalButtons()
        {
            _scaleform = Scaleform.RequestMovie("instructional_buttons");
            _buttons = new List<InstructionalButton>();
            _needsUpdate = true;
            _lastInputMethod = -1;
        }

        /// <summary>Adds a button by control and label.</summary>
        public void Add(Control control, string text)
        {
            _buttons.Add(new InstructionalButton(control, text));
            _needsUpdate = true;
        }

        /// <summary>Adds a pre-constructed button instance.</summary>
        public void Add(InstructionalButton button)
        {
            _buttons.Add(button);
            _needsUpdate = true;
        }

        /// <summary>Removes all buttons matching the given control.</summary>
        public void Remove(Control control)
        {
            _buttons.RemoveAll(b => b.Control == control);
            _needsUpdate = true;
        }

        /// <summary>Removes all buttons.</summary>
        public void Clear()
        {
            _buttons.Clear();
            _needsUpdate = true;
        }

        /// <summary>
        /// Draws the button row this frame. Call once per OnTick.
        /// Rebuilds the scaleform if buttons changed or the input method switched
        /// (keyboard ↔ controller), so glyphs always match the current device.
        /// </summary>
        public void Draw()
        {
            int currentInputMethod = Function.Call<int>(GetLastInputMethodHash, 2);

            if (currentInputMethod != _lastInputMethod)
            {
                _needsUpdate = true;
                _lastInputMethod = currentInputMethod;
            }

            if (_needsUpdate)
                Rebuild();

            _scaleform.Render2D();
        }

        public void Dispose()
        {
            _scaleform?.Dispose();
        }

        // ─────────────────────────────────────────────────────────────────────
        // Private
        // ─────────────────────────────────────────────────────────────────────

        /// <summary>
        /// Pushes the current button list into the scaleform.
        /// Called automatically by Draw() when the list or input method changes.
        /// </summary>
        private void Rebuild()
        {
            _scaleform.CallFunction("CLEAR_ALL");
            _scaleform.CallFunction("TOGGLE_MOUSE_BUTTONS", false);
            _scaleform.CallFunction("CREATE_CONTAINER");

            for (int i = 0; i < _buttons.Count; i++)
            {
                var button = _buttons[i];
                _scaleform.CallFunction("SET_DATA_SLOT", i, button.GetButtonId(), button.Text);
            }

            _scaleform.CallFunction("DRAW_INSTRUCTIONAL_BUTTONS", -1);
            _needsUpdate = false;
        }
    }
}
