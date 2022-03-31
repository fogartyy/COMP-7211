using SlotCode;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Media.SpeechSynthesis;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace UI
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        Slots slot;
        SpeechSynthesizer speechSynthesizer;


        MediaElement mediaElement = new MediaElement();

        public MainPage()
        {
            this.InitializeComponent();
            

            speechSynthesizer = new SpeechSynthesizer();
            Slider.AddHandler(UIElement.PointerReleasedEvent, new PointerEventHandler(UIElement_OnPointerReleased), true);
            Dictionary<int, Image> Wheels = new Dictionary<int, Image>()
            {
                { 1,Img1},
                { 2,Img2},
                { 3,Img3}
            };

            slot = new Slots(Wheels, TextInfo, speechSynthesizer, mediaElement);

        }



        private void AddMoney(object sender, RoutedEventArgs e)
        {
            textBlockDollars.Text = slot.AddMoney();
        }

        private void imageWheel1_Click(object sender, RoutedEventArgs e)
        {
            textBlockDollars.Text = slot.Hold(1);
        }

        private void imageWheel2_Click(object sender, RoutedEventArgs e)
        {
            textBlockDollars.Text = slot.Hold(2);
        }

        private void imageWheel3_Click(object sender, RoutedEventArgs e)
        {
            textBlockDollars.Text = slot.Hold(3);
        }

        private void UIElement_OnPointerReleased(object sender, PointerRoutedEventArgs e)
        {
            if (Slider.Value == 100)
            {
                Slider.Value = 0;
                textBlockDollars.Text = slot.SpinWheel();
            }
            else
            {
                Slider.Value = 0;
            }
        }
    }
}
