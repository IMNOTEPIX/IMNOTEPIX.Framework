using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using GTA;
using GTA.Native;

namespace IMNOTEPIX.Framework.fUI.Scaleforms
{
    public class InstructionalButtons
    {
        private const string ScaleformName = "instructional_buttons";

        public static readonly Color DefaultBackgroundColor = Color.FromArgb(80, 0, 0, 0);

        private bool needsUpdate = true;

        public Scaleform Scaleform { get; }
        public InstructionalButtonsCollection Buttons { get; }
        public Color BackgroundColor { get; set; } = DefaultBackgroundColor;
        public float MaxWidth { get; set; } = 1.0f;
        public bool MouseButtonsEnabled { get; set; }

        public InstructionalButtons()
        {
            // In SHVDN, the Scaleform constructor takes the name and begins loading immediately.
            Scaleform = Scaleform.RequestMovieIgnoreSuperWidescreenAdjustment(ScaleformName);

            Buttons = new InstructionalButtonsCollection();
            Buttons.ItemAdded += (c, i) => Update();
            Buttons.ItemRemoved += (c, i) => Update();
            Buttons.ItemModified += (c, o, n) => Update();
            Buttons.Cleared += (c) => Update();
        }

        public void Draw()
        {
            if (needsUpdate || Function.Call<bool>(Hash.HAVE_CONTROLS_CHANGED, 2)) // recheck when player switches keyboard/controller
            {
                DoUpdate();
            }

            Scaleform.Render2D();
        }

        public void Update() => needsUpdate = true;

        private void DoUpdate()
        {
            // Still loading — try again next tick.
            if (!Scaleform.IsLoaded) 
                return;

            Scaleform.CallFunction("CLEAR_ALL");
            Scaleform.CallFunction("TOGGLE_MOUSE_BUTTONS", MouseButtonsEnabled);
            Scaleform.CallFunction("SET_MAX_WIDTH", MaxWidth);

            for (int i = 0, slot = 0; i < Buttons.Count; i++)
            {
                IInstructionalButtonSlot b = Buttons[i];
                if (b.CanBeDisplayed == null || b.CanBeDisplayed(b))
                {
                    if (MouseButtonsEnabled)
                    {
                        Scaleform.CallFunction("SET_DATA_SLOT", slot++, b.GetButtonId() ?? string.Empty, b.Text ?? string.Empty, b.BindedControl.HasValue, b.BindedControl.HasValue ? (int)b.BindedControl.Value : -1);
                    }
                    else
                    {
                        Scaleform.CallFunction("SET_DATA_SLOT", slot++, b.GetButtonId() ?? string.Empty, b.Text ?? string.Empty);
                    }
                }
            }

            Scaleform.CallFunction("SET_BACKGROUND_COLOUR", (int)BackgroundColor.R, (int)BackgroundColor.G, (int)BackgroundColor.B, (int)BackgroundColor.A);
            Scaleform.CallFunction("DRAW_INSTRUCTIONAL_BUTTONS", 0);

            needsUpdate = false;
        }

        /// <summary>
        /// Returns the number of rows currently occupied by the instructional buttons scaleform.
        /// Simplified from the original: the RAGE version used internal memory access
        /// (CBusySpinner, CScaleformMgr) which is not available in SHVDN.
        /// </summary>
        internal static int NumberOfRows 
            => Function.Call<bool>(Hash.BUSYSPINNER_IS_ON) ? 1 : 0;


        public class InstructionalButtonsCollection : BaseCollection<IInstructionalButtonSlot>, IEnumerable<InstructionalButton>
        {
            // Backwards-compatibility: allow LINQ to work on InstructionalButton entries
            // even though the list now holds the broader IInstructionalButtonSlot interface.
            [EditorBrowsable(EditorBrowsableState.Never)]
            IEnumerator<InstructionalButton> IEnumerable<InstructionalButton>.GetEnumerator()
                => InternalList.OfType<InstructionalButton>().GetEnumerator();
        }
    }


    // ---------------------------------------------------------------------------
    // BaseCollection<T>
    // Replaces the RAGE-specific BaseCollection<T>. Consider moving to Core/ if
    // you need it elsewhere in the framework.
    // ---------------------------------------------------------------------------
    public class BaseCollection<T> : IEnumerable<T>
    {
        protected readonly List<T> InternalList = new List<T>();

        public event Action<BaseCollection<T>, T> ItemAdded;
        public event Action<BaseCollection<T>, T> ItemRemoved;
        public event Action<BaseCollection<T>, T, T> ItemModified;
        public event Action<BaseCollection<T>> Cleared;

        public int Count => InternalList.Count;

        public T this[int index]
        {
            get => InternalList[index];
            set
            {
                T old = InternalList[index];
                InternalList[index] = value;
                ItemModified?.Invoke(this, old, value);
            }
        }

        public void Add(T item)
        {
            InternalList.Add(item);
            ItemAdded?.Invoke(this, item);
        }

        public bool Remove(T item)
        {
            bool removed = InternalList.Remove(item);
            if (removed) ItemRemoved?.Invoke(this, item);
            return removed;
        }

        public void Clear()
        {
            InternalList.Clear();
            Cleared?.Invoke(this);
        }

        public IEnumerator<T> GetEnumerator() => InternalList.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }


    // ---------------------------------------------------------------------------
    // Interfaces & button types
    // ---------------------------------------------------------------------------

    public interface IInstructionalButtonSlot
    {
        /// <summary>Text displayed next to the button.</summary>
        public string Text { get; set; }

        /// <summary>
        /// Optional predicate controlling visibility.
        /// Evaluated when <see cref="InstructionalButtons.Update"/> is called.
        /// </summary>
        public Predicate<IInstructionalButtonSlot> CanBeDisplayed { get; set; }

        /// <summary>
        /// The <see cref="Control"/> triggered when the button is clicked.
        /// If <c>null</c>, the button is not clickable.
        /// </summary>
        public Control? BindedControl { get; set; }

        /// <summary>Returns the scaleform button ID string for this slot.</summary>
        public string GetButtonId();
    }

    public class InstructionalButton : IInstructionalButtonSlot
    {
        /// <inheritdoc/>
        public string Text { get; set; }

        /// <summary>The button symbol / identifier to display.</summary>
        public InstructionalButtonId Button { get; set; }

        /// <inheritdoc/>
        public Predicate<IInstructionalButtonSlot> CanBeDisplayed { get; set; }

        /// <inheritdoc/>
        public Control? BindedControl { get; set; }

        public InstructionalButton(InstructionalButtonId button, string text)
        {
            Text = text;
            Button = button;
        }

        /// <param name="control">Adapts to keybinds and keyboard/controller state automatically.</param>
        public InstructionalButton(Control control, string text)
            : this((InstructionalButtonId)control, text) { BindedControl = control; }

        /// <param name="buttonText">Custom key label, e.g. "F5" or "I".</param>
        public InstructionalButton(string buttonText, string text)
            : this((InstructionalButtonId)buttonText, text) { }

        public InstructionalButton(uint rawId, string text)
            : this((InstructionalButtonId)rawId, text) { }

        public InstructionalButton(InstructionalKey key, string text)
            : this((InstructionalButtonId)key, text) { }

        /// <inheritdoc/>
        public string GetButtonId() => Button.Id;

        public static string GetButtonId(Control control) => new InstructionalButtonId(control).Id;
        public static string GetButtonId(string keyString) => new InstructionalButtonId(keyString).Id;
        public static string GetButtonId(uint rawId) => new InstructionalButtonId(rawId).Id;
        public static string GetButtonId(InstructionalKey key) => new InstructionalButtonId(key).Id;
    }

    public class InstructionalButtonGroup : IInstructionalButtonSlot
    {
        private IList<InstructionalButtonId> buttons;

        /// <inheritdoc/>
        public string Text { get; set; }

        /// <inheritdoc/>
        public Predicate<IInstructionalButtonSlot> CanBeDisplayed { get; set; }

        /// <inheritdoc/>
        public Control? BindedControl { get; set; }

        /// <summary>The buttons shown in this group.</summary>
        /// <exception cref="ArgumentNullException">Thrown when set to null.</exception>
        public IList<InstructionalButtonId> Buttons
        {
            get => buttons;
            set => buttons = value ?? throw new ArgumentNullException(nameof(value));
        }

        public InstructionalButtonGroup(string text, IEnumerable<InstructionalButtonId> buttons)
        {
            Text = text;
            Buttons = new List<InstructionalButtonId>(buttons);
        }

        public InstructionalButtonGroup(string text, params InstructionalButtonId[] buttons)
            : this(text, (IEnumerable<InstructionalButtonId>)buttons) { }

        /// <inheritdoc/>
        public string GetButtonId() => GetButtonId(Buttons);

        public static string GetButtonId(IEnumerable<InstructionalButtonId> buttons)
            => string.Join("%", buttons.Reverse().Select(btn => btn.Id));
    }

    public class InstructionalButtonDynamic : IInstructionalButtonSlot
    {
        private IList<InstructionalButtonId> buttonsForKeyboard, buttonsForController;

        /// <inheritdoc/>
        public string Text { get; set; }

        public IList<InstructionalButtonId> ButtonsForKeyboard
        {
            get => buttonsForKeyboard;
            set => buttonsForKeyboard = value ?? throw new ArgumentNullException(nameof(value));
        }

        public IList<InstructionalButtonId> ButtonsForController
        {
            get => buttonsForController;
            set => buttonsForController = value ?? throw new ArgumentNullException(nameof(value));
        }

        /// <inheritdoc/>
        public Predicate<IInstructionalButtonSlot> CanBeDisplayed { get; set; }

        /// <inheritdoc/>
        public Control? BindedControl { get; set; }

        public InstructionalButtonDynamic(string text, InstructionalButtonId forKeyboard, InstructionalButtonId forController)
        {
            Text = text;
            ButtonsForKeyboard = new List<InstructionalButtonId>(1) { forKeyboard };
            ButtonsForController = new List<InstructionalButtonId>(1) { forController };
        }

        public InstructionalButtonDynamic(string text, IEnumerable<InstructionalButtonId> forKeyboard, IEnumerable<InstructionalButtonId> forController)
        {
            Text = text;
            ButtonsForKeyboard = new List<InstructionalButtonId>(forKeyboard);
            ButtonsForController = new List<InstructionalButtonId>(forController);
        }

        // IS_USING_KEYBOARD_AND_MOUSE returns true when on KB+M, so negate for controller check.
        private static bool IsUsingController 
            => !Function.Call<bool>(Hash.IS_USING_KEYBOARD_AND_MOUSE, 2);

        /// <inheritdoc/>
        public string GetButtonId() 
            => IsUsingController ? InstructionalButtonGroup.GetButtonId(ButtonsForController) : InstructionalButtonGroup.GetButtonId(ButtonsForKeyboard);
    }


    // ---------------------------------------------------------------------------
    // InstructionalButtonId
    // ---------------------------------------------------------------------------

    public readonly struct InstructionalButtonId
    {
        private const string EmptyButtonId = "t_";

        // Store the control separately so the ID string can be regenerated dynamically
        // when the player switches between keyboard and controller.
        private readonly Control? control;
        private readonly string id;

        public string Id => id ?? (control.HasValue ? Function.Call<string>(Hash.GET_CONTROL_INSTRUCTIONAL_BUTTONS_STRING, 2, (int)control.Value, true) : EmptyButtonId);

        /// <param name="control">Adapts automatically to the player's current input device.</param>
        public InstructionalButtonId(Control control) : this() => this.control = control;

        /// <param name="str">Custom key label (e.g. "I", "F5"). Length determines the size prefix.</param>
        public InstructionalButtonId(string str) : this()
            => id = str.Length switch
            {
                int n when n <= 2 => "t_",
                int n when n <= 4 => "T_",
                _ => "w_"
            } + str;

        /// <param name="rawId">Raw symbol identifier.</param>
        public InstructionalButtonId(uint rawId) : this() => id = "b_" + rawId;

        /// <param name="key">A well-known instructional key symbol.</param>
        public InstructionalButtonId(InstructionalKey key) : this() => id = key.GetId();

        public static implicit operator InstructionalButtonId(Control control) => new InstructionalButtonId(control);
        public static implicit operator InstructionalButtonId(string str) => new InstructionalButtonId(str);
        public static implicit operator InstructionalButtonId(uint rawId) => new InstructionalButtonId(rawId);
        public static implicit operator InstructionalButtonId(InstructionalKey key) => new InstructionalButtonId(key);
    }
}
