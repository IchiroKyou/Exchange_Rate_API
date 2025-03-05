﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace ExchangeRateApi.Resources {
    using System;
    
    
    /// <summary>
    ///   A strongly-typed resource class, for looking up localized strings, etc.
    /// </summary>
    // This class was auto-generated by the StronglyTypedResourceBuilder
    // class via a tool like ResGen or Visual Studio.
    // To add or remove a member, edit your .ResX file then rerun ResGen
    // with the /str option, or rebuild your VS project.
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "17.0.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    internal class ApiMessages {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal ApiMessages() {
        }
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("ExchangeRateApi.Resources.ApiMessages", typeof(ApiMessages).Assembly);
                    resourceMan = temp;
                }
                return resourceMan;
            }
        }
        
        /// <summary>
        ///   Overrides the current thread's CurrentUICulture property for all
        ///   resource lookups using this strongly typed resource class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Globalization.CultureInfo Culture {
            get {
                return resourceCulture;
            }
            set {
                resourceCulture = value;
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Error fetching exchange rate: {0}.
        /// </summary>
        internal static string Error_AlphaVantage_HttpRequest {
            get {
                return ResourceManager.GetString("Error_AlphaVantage_HttpRequest", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Exchange for {0}/{1} was not found..
        /// </summary>
        internal static string Error_AlphaVantage_NotFound {
            get {
                return ResourceManager.GetString("Error_AlphaVantage_NotFound", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Error parsing JSON: {0}.
        /// </summary>
        internal static string Error_AlphaVantage_ParsingJson {
            get {
                return ResourceManager.GetString("Error_AlphaVantage_ParsingJson", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to An unexpected error occurred: {0}.
        /// </summary>
        internal static string Error_AlphaVantage_UnexpectedException {
            get {
                return ResourceManager.GetString("Error_AlphaVantage_UnexpectedException", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The API key for {0} was not found in the environment variable!.
        /// </summary>
        internal static string Error_ApiKeyNotFound {
            get {
                return ResourceManager.GetString("Error_ApiKeyNotFound", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Key {0} in appsettings not found!.
        /// </summary>
        internal static string Error_EnvKeyNotFound {
            get {
                return ResourceManager.GetString("Error_EnvKeyNotFound", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The exchange rate between {0} and {1} was not found!.
        /// </summary>
        internal static string Exception_ExchangeRateNotFound_Message {
            get {
                return ResourceManager.GetString("Exception_ExchangeRateNotFound_Message", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Invalid currency code format. Must be 3 letters.a.
        /// </summary>
        internal static string Validation_CurrencyCode_InvalidCodeFormat {
            get {
                return ResourceManager.GetString("Validation_CurrencyCode_InvalidCodeFormat", resourceCulture);
            }
        }
    }
}
