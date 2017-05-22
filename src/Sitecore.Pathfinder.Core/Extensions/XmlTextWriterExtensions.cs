// � 2015-2017 Sitecore Corporation A/S. All rights reserved.

using System.Xml;
using Sitecore.Pathfinder.Diagnostics;

namespace Sitecore.Pathfinder.Extensions
{
    public static class XmlTextWriterExtensions
    {
        public static void WriteAttributeStringIf([NotNull] this XmlWriter textWriter, [NotNull] string localName, [NotNull] string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                return;
            }

            textWriter.WriteAttributeString(localName, value);
        }

        public static void WriteAttributeStringIf([NotNull] this XmlWriter textWriter, [NotNull] string localName, int value, int defaultValue = 0)
        {
            if (value == defaultValue)
            {
                return;
            }

            textWriter.WriteAttributeString(localName, value.ToString());
        }

        public static void WriteAttributeStringIf([NotNull] this XmlWriter textWriter, [NotNull] string localName, bool value, bool defaultValue = false)
        {
            if (value == defaultValue)
            {
                return;
            }

            textWriter.WriteAttributeString(localName, value ? "True" : "False");
        }

        public static void WriteFullElementString([NotNull] this XmlWriter textWriter, [NotNull] string localName, [NotNull] string value)
        {
            if (!string.IsNullOrEmpty(value))
            {
                textWriter.WriteElementString(localName, value);
                return;
            }

            textWriter.WriteStartElement(localName);
            textWriter.WriteFullEndElement();
        }
    }
}
