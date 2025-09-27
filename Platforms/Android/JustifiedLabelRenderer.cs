
using Android.Text;

using Android.Widget;
using AndroidX.Core.Text;
using Microsoft.Maui.Handlers;
using Android.Views;


namespace X10Card.Platforms.Android
{

    public class JustifiedLabelHandler : LabelHandler
    {
        public static IPropertyMapper<JustifiedLabel, JustifiedLabelHandler> JustifiedLabelMapper =
            new PropertyMapper<JustifiedLabel, JustifiedLabelHandler>(LabelHandler.Mapper)
            {
                [nameof(Label.Text)] = MapText
            };

        public JustifiedLabelHandler() : base(JustifiedLabelMapper)
        {
        }

        public static void MapText(JustifiedLabelHandler handler, JustifiedLabel label)
        {
            if (handler.PlatformView is TextView textView)
            {
                if (!string.IsNullOrWhiteSpace(label.Text))
                {
                    textView.SetText(
                        HtmlCompat.FromHtml(label.Text, HtmlCompat.FromHtmlModeLegacy),
                        TextView.BufferType.Spannable);

                    // Apply justification based on Android SDK version
                    if (OperatingSystem.IsAndroidVersionAtLeast(26))
                    {
                        textView.JustificationMode = JustificationMode.InterWord;
                    }
                    else
                    {
                        // Fallback for older APIs
                        textView.Gravity = GravityFlags.FillHorizontal | GravityFlags.CenterVertical;
                    }

                    // Set line spacing
                    textView.SetLineSpacing(6f, 1.3f);

                    // Ensure text can wrap and doesn't get ellipsized
                    textView.SetSingleLine(false);
                    textView.Ellipsize = null;
                    textView.SetHorizontallyScrolling(false);

                    // Removed the manual re-measurement and layout code.
                    // This was the cause of the text overlapping.
                    // The native layout system will now handle the size correctly.
                }
                else
                {
                    textView.Text = "";
                }
            }
        }
    }
}