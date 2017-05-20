﻿// © 2015-2017 Sitecore Corporation A/S. All rights reserved.

using System.Globalization;
using System.Reflection;
using System.Resources;

#pragma warning disable RNUL // Field is missing nullability annotation.
#pragma warning disable RINUL // Parameter is missing item nullability annotation.

namespace Sitecore.Pathfinder.Configuration.ConfigurationModel
{
    internal static class Resources
    {
        private static readonly ResourceManager _resourceManager = new ResourceManager("Microsoft.Framework.ConfigurationModel.Resources", typeof(Resources).GetTypeInfo().Assembly);

        /// <summary>
        /// Unable to commit because the following keys are missing from the configuration file: {0}.
        /// </summary>
        internal static string Error_CommitWhenKeyMissing
        {
            get { return GetString("Error_CommitWhenKeyMissing"); }
        }

        /// <summary>
        /// Unable to commit because a new key was added to the configuration file after last load operation. The newly added key is '{0}'.
        /// </summary>
        internal static string Error_CommitWhenNewKeyFound
        {
            get { return GetString("Error_CommitWhenNewKeyFound"); }
        }

        /// <summary>
        /// Keys in switch mappings are case-insensitive. A duplicated key '{0}' was found.
        /// </summary>
        internal static string Error_DuplicatedKeyInSwitchMappings
        {
            get { return GetString("Error_DuplicatedKeyInSwitchMappings"); }
        }

        /// <summary>
        /// The configuration file '{0}' was not found and is not optional.
        /// </summary>
        internal static string Error_FileNotFound
        {
            get { return GetString("Error_FileNotFound"); }
        }

        /// <summary>File path must be a non-empty string.</summary>
        internal static string Error_InvalidFilePath
        {
            get { return GetString("Error_InvalidFilePath"); }
        }

        /// <summary>The switch mappings contain an invalid switch '{0}'.</summary>
        internal static string Error_InvalidSwitchMapping
        {
            get { return GetString("Error_InvalidSwitchMapping"); }
        }

        /// <summary>A duplicate key '{0}' was found.</summary>
        internal static string Error_KeyIsDuplicated
        {
            get { return GetString("Error_KeyIsDuplicated"); }
        }

        /// <summary>
        /// No registered configuration source is capable of committing changes.
        /// </summary>
        internal static string Error_NoCommitableSource
        {
            get { return GetString("Error_NoCommitableSource"); }
        }

        /// <summary>
        /// The short switch '{0}' is not defined in the switch mappings.
        /// </summary>
        internal static string Error_ShortSwitchNotDefined
        {
            get { return GetString("Error_ShortSwitchNotDefined"); }
        }

        /// <summary>Unrecognized argument format: '{0}'.</summary>
        internal static string Error_UnrecognizedArgumentFormat
        {
            get { return GetString("Error_UnrecognizedArgumentFormat"); }
        }

        /// <summary>Unrecognized line format: '{0}'.</summary>
        internal static string Error_UnrecognizedLineFormat
        {
            get { return GetString("Error_UnrecognizedLineFormat"); }
        }

        /// <summary>Value for switch '{0}' is missing.</summary>
        internal static string Error_ValueIsMissing
        {
            get { return GetString("Error_ValueIsMissing"); }
        }

        /// <summary>
        /// Unable to commit because the following keys are missing from the configuration file: {0}.
        /// </summary>
        internal static string FormatError_CommitWhenKeyMissing(object p0)
        {
            return string.Format(CultureInfo.CurrentCulture, GetString("Error_CommitWhenKeyMissing"), new object[1]
            {
                p0
            });
        }

        /// <summary>
        /// Unable to commit because a new key was added to the configuration file after last load operation. The newly added key is '{0}'.
        /// </summary>
        internal static string FormatError_CommitWhenNewKeyFound(object p0)
        {
            return string.Format(CultureInfo.CurrentCulture, GetString("Error_CommitWhenNewKeyFound"), new object[1]
            {
                p0
            });
        }

        /// <summary>
        /// Keys in switch mappings are case-insensitive. A duplicated key '{0}' was found.
        /// </summary>
        internal static string FormatError_DuplicatedKeyInSwitchMappings(object p0)
        {
            return string.Format(CultureInfo.CurrentCulture, GetString("Error_DuplicatedKeyInSwitchMappings"), new object[1]
            {
                p0
            });
        }

        /// <summary>
        /// The configuration file '{0}' was not found and is not optional.
        /// </summary>
        internal static string FormatError_FileNotFound(object p0)
        {
            return string.Format(CultureInfo.CurrentCulture, GetString("Error_FileNotFound"), new object[1]
            {
                p0
            });
        }

        /// <summary>File path must be a non-empty string.</summary>
        internal static string FormatError_InvalidFilePath()
        {
            return GetString("Error_InvalidFilePath");
        }

        /// <summary>The switch mappings contain an invalid switch '{0}'.</summary>
        internal static string FormatError_InvalidSwitchMapping(object p0)
        {
            return string.Format(CultureInfo.CurrentCulture, GetString("Error_InvalidSwitchMapping"), new object[1]
            {
                p0
            });
        }

                            /// <summary>A duplicate key '{0}' was found.</summary>
        internal static string FormatError_KeyIsDuplicated(object p0)
        {
            return string.Format(CultureInfo.CurrentCulture, GetString("Error_KeyIsDuplicated"), new object[1]
            {
                p0
            });
        }

        /// <summary>
        /// No registered configuration source is capable of committing changes.
        /// </summary>
        internal static string FormatError_NoCommitableSource()
        {
            return GetString("Error_NoCommitableSource");
        }

        /// <summary>
        /// The short switch '{0}' is not defined in the switch mappings.
        /// </summary>
        internal static string FormatError_ShortSwitchNotDefined(object p0)
        {
            return string.Format(CultureInfo.CurrentCulture, GetString("Error_ShortSwitchNotDefined"), new object[1]
            {
                p0
            });
        }

        /// <summary>Unrecognized argument format: '{0}'.</summary>
        internal static string FormatError_UnrecognizedArgumentFormat(object p0)
        {
            return string.Format(CultureInfo.CurrentCulture, GetString("Error_UnrecognizedArgumentFormat"), new object[1]
            {
                p0
            });
        }

        /// <summary>Unrecognized line format: '{0}'.</summary>
        internal static string FormatError_UnrecognizedLineFormat(object p0)
        {
            return string.Format(CultureInfo.CurrentCulture, GetString("Error_UnrecognizedLineFormat"), new object[1]
            {
                p0
            });
        }

        /// <summary>Value for switch '{0}' is missing.</summary>
        internal static string FormatError_ValueIsMissing(object p0)
        {
            return string.Format(CultureInfo.CurrentCulture, GetString("Error_ValueIsMissing"), new object[1]
            {
                p0
            });
        }

        private static string GetString(string name, params string[] formatterNames)
        {
            var str = _resourceManager.GetString(name);
            if (formatterNames != null)
            {
                for (var index = 0; index < formatterNames.Length; ++index)
                {
                    str = str.Replace("{" + formatterNames[index] + "}", "{" + index + "}");
                }
            }

            return str;
        }
    }
}
