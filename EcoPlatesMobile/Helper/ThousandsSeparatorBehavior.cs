using System.Globalization;

namespace EcoPlatesMobile.Helper
{ 
    public class ThousandsSeparatorBehavior : Behavior<Entry>
    {
        bool _isFormatting;
        Entry _entry;

        public static readonly BindableProperty RawValueProperty =
            BindableProperty.Create(
                nameof(RawValue),
                typeof(long?),
                typeof(ThousandsSeparatorBehavior),
                default(long?),
                BindingMode.TwoWay,
                propertyChanged: OnRawValueChanged);

        public long? RawValue
        {
            get => (long?)GetValue(RawValueProperty);
            set => SetValue(RawValueProperty, value);
        }

        protected override void OnAttachedTo(Entry bindable)
        {
            base.OnAttachedTo(bindable);
            _entry = bindable;
            _entry.TextChanged += OnTextChanged;
        }

        protected override void OnDetachingFrom(Entry bindable)
        {
            base.OnDetachingFrom(bindable);
            _entry.TextChanged -= OnTextChanged;
            _entry = null;
        }

        static void OnRawValueChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var behavior = (ThousandsSeparatorBehavior)bindable;
            if (behavior._entry == null || behavior._isFormatting) return;

            try
            {
                behavior._isFormatting = true;

                if (newValue is long l)
                {
                    var formatted = Format(l);
                    behavior._entry.Text = formatted;
                }
                else
                {
                    behavior._entry.Text = string.Empty;
                }
            }
            finally
            {
                behavior._isFormatting = false;
            }
        }

        void OnTextChanged(object sender, TextChangedEventArgs e)
        {
            if (_isFormatting) return;

            try
            {
                _isFormatting = true;

                // Remember caret position relative to the end
                var oldText = e.OldTextValue ?? string.Empty;
                var newText = e.NewTextValue ?? string.Empty;
                var oldLen = oldText.Length;
                var newLen = newText.Length;
                var cursorFromEnd = (oldLen - (_entry.CursorPosition >= 0 ? _entry.CursorPosition : oldLen));

                // Keep only digits
                var digits = new string(newText.Where(char.IsDigit).ToArray());

                if (string.IsNullOrEmpty(digits))
                {
                    RawValue = null;
                    _entry.Text = string.Empty;
                    _entry.CursorPosition = 0;
                    return;
                }

                if (!long.TryParse(digits, NumberStyles.None, CultureInfo.InvariantCulture, out var value))
                {
                    // If too large to parse as long, just revert to old text
                    _entry.Text = e.OldTextValue;
                    return;
                }

                RawValue = value;

                var formatted = Format(value);
                _entry.Text = formatted;

                // Recompute caret aiming to preserve distance from the end by digits count
                var newFormattedLen = formatted.Length;
                var newCursorPos = Math.Max(0, newFormattedLen - cursorFromEnd);
                if (newCursorPos <= newFormattedLen)
                    _entry.CursorPosition = newCursorPos;
                else
                    _entry.CursorPosition = newFormattedLen;
            }
            finally
            {
                _isFormatting = false;
            }
        }

        static string Format(long value)
        {
            // Culture-aware grouping (e.g., "1,234" for en-US, "1 234" for fr-FR, etc.)
            //return value.ToString("N0", CultureInfo.CurrentCulture);
            return value.ToString("#,0", CultureInfo.CurrentCulture);
        }
    }
}