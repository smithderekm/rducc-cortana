﻿#pragma checksum "E:\workspace\RDUCC15\CortanaLightControl\CortanaLightListener\MainPage.xaml" "{406ea660-64cf-4c82-b6f0-42d48172a799}" "03C56BDFED2EE94A2CBE74553E446850"
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace CortanaLightController
{
    partial class MainPage : 
        global::Windows.UI.Xaml.Controls.Page, 
        global::Windows.UI.Xaml.Markup.IComponentConnector,
        global::Windows.UI.Xaml.Markup.IComponentConnector2
    {
        /// <summary>
        /// Connect()
        /// </summary>
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Windows.UI.Xaml.Build.Tasks"," 14.0.0.0")]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        public void Connect(int connectionId, object target)
        {
            switch(connectionId)
            {
            case 1:
                {
                    this.textBlock = (global::Windows.UI.Xaml.Controls.TextBlock)(target);
                }
                break;
            case 2:
                {
                    this.LightOnButton = (global::Windows.UI.Xaml.Controls.Button)(target);
                    #line 17 "..\..\..\MainPage.xaml"
                    ((global::Windows.UI.Xaml.Controls.Button)this.LightOnButton).Click += this.LightOnButton_Click;
                    #line default
                }
                break;
            case 3:
                {
                    this.LightOffButton = (global::Windows.UI.Xaml.Controls.Button)(target);
                    #line 18 "..\..\..\MainPage.xaml"
                    ((global::Windows.UI.Xaml.Controls.Button)this.LightOffButton).Click += this.LightOffButton_Click;
                    #line default
                }
                break;
            case 4:
                {
                    this.image = (global::Windows.UI.Xaml.Controls.Image)(target);
                }
                break;
            case 5:
                {
                    this.toggleSwitchKitchen = (global::Windows.UI.Xaml.Controls.ToggleSwitch)(target);
                    #line 20 "..\..\..\MainPage.xaml"
                    ((global::Windows.UI.Xaml.Controls.ToggleSwitch)this.toggleSwitchKitchen).Toggled += this.toggleSwitchKitchen_Toggled;
                    #line default
                }
                break;
            case 6:
                {
                    this.toggleSwitchOffice = (global::Windows.UI.Xaml.Controls.ToggleSwitch)(target);
                    #line 21 "..\..\..\MainPage.xaml"
                    ((global::Windows.UI.Xaml.Controls.ToggleSwitch)this.toggleSwitchOffice).Toggled += this.toggleSwitchOffice_Toggled;
                    #line default
                }
                break;
            case 7:
                {
                    this.toggleSwitchBedroom = (global::Windows.UI.Xaml.Controls.ToggleSwitch)(target);
                    #line 22 "..\..\..\MainPage.xaml"
                    ((global::Windows.UI.Xaml.Controls.ToggleSwitch)this.toggleSwitchBedroom).Toggled += this.toggleSwitchBedroom_Toggled;
                    #line default
                }
                break;
            case 8:
                {
                    this.outputScroller = (global::Windows.UI.Xaml.Controls.ScrollViewer)(target);
                }
                break;
            case 9:
                {
                    this.outputContent = (global::Windows.UI.Xaml.Controls.StackPanel)(target);
                }
                break;
            default:
                break;
            }
            this._contentLoaded = true;
        }

        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Windows.UI.Xaml.Build.Tasks"," 14.0.0.0")]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        public global::Windows.UI.Xaml.Markup.IComponentConnector GetBindingConnector(int connectionId, object target)
        {
            global::Windows.UI.Xaml.Markup.IComponentConnector returnValue = null;
            return returnValue;
        }
    }
}

