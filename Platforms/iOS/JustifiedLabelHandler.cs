using Foundation;
using Microsoft.Maui.Handlers;
using UIKit;

namespace X10Card.Platforms.iOS
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
            if (handler.PlatformView is UILabel uiLabel)
            {
                if (!string.IsNullOrWhiteSpace(label.Text))
                {
                    // Create a mutable attributed string from the label's text
                    var attributedString = new NSMutableAttributedString(label.Text);

                    // Create a paragraph style for justification and line spacing
                    var paragraphStyle = new NSMutableParagraphStyle();

                    // Check for iOS 13.0 or later for native justification
                    if (UIDevice.CurrentDevice.CheckSystemVersion(13, 0))
                    {
                        paragraphStyle.Alignment = UITextAlignment.Justified;
                    }
                    else
                    {
                        // Fallback for older iOS versions, as there is no native justification
                        paragraphStyle.Alignment = UITextAlignment.Left;
                    }

                    // Set line spacing, equivalent to Android's SetLineSpacing
                    // This creates a 1.3x line height with a 6-point space between lines
                    paragraphStyle.LineSpacing = (nfloat)(label.LineHeight * 1.3f) + 6f;

                    // Add the paragraph style to the attributed string's attributes
                    var range = new NSRange(0, attributedString.Length);
                    attributedString.AddAttribute(UIStringAttributeKey.ParagraphStyle, paragraphStyle, range);

                    // Apply the attributed string to the UILabel
                    uiLabel.AttributedText = attributedString;
                }
                else
                {
                    uiLabel.Text = "";
                }
            }
        }
    }
}
