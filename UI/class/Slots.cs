using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Media.SpeechSynthesis;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;

namespace SlotCode
{

    class Slots
    {

        TextBlock Textinfo;

        int money = 0;
        Random rand = new Random();
        Dictionary<int, DataClass> Wheel = new Dictionary<int, DataClass>()
        {
            {1, new DataClass()},
            {2, new DataClass()},
            {3, new DataClass()}

        };
        

        Dictionary<int, Payout> PayoutData = new Dictionary<int, Payout>()
        {
            {3072, new Payout(600)},
            {768, new Payout(500)},
            {192, new Payout(400)},
            {48, new Payout(300)},
            {12, new Payout(200)},
            {1536, new Payout(100)},
            {1152, new Payout(80)},
            {1056, new Payout(60)},
            {1032, new Payout(40)}
        };

        int[] BinaryNum = new int[6]
        {
            1,
            4,
            16,
            64,
            256,
            1024
        };

        BitmapImage[] Images = new BitmapImage[6] 
        {
            new BitmapImage(new Uri("ms-appx:///images/slots/Lose.png")),
            new BitmapImage(new Uri("ms-appx:///images/slots/Clubs.png")),
            new BitmapImage(new Uri("ms-appx:///images/slots/Spade.png")),
            new BitmapImage(new Uri("ms-appx:///images/slots/Heart.png")),
            new BitmapImage(new Uri("ms-appx:///images/slots/Diamonds.png")),
            new BitmapImage(new Uri("ms-appx:///images/slots/Win.png"))
        };

        SpeechSynthesizer speechSynthesizer;
        MediaElement mediaElement;
        public Slots(Dictionary<int,Image> Wheels, TextBlock text, SpeechSynthesizer SpeechSynthesizer,
        MediaElement MediaElement)
        {
            Textinfo = text;
            foreach (var data in Wheels)
            {
                Wheel[data.Key].ButtonRef = data.Value;
            }

            speechSynthesizer = SpeechSynthesizer;
            mediaElement = MediaElement;
        }

        public string AddMoney()
        {
            money += 10;
            return $"${money}";
        }


        public async void Speak(string Text)
        {
            try
            {
                var stream = await speechSynthesizer.SynthesizeTextToStreamAsync(Text);
                mediaElement.SetSource(stream, stream.ContentType);
                mediaElement.Play();
            }
            catch (Exception)
            {
                Debug.WriteLine("Speech not working");
            }
            
        }

        public string SpinWheel()
        {

            Textinfo.Text = "";
            if (money - 10 >= 0)
            {
                money = money - ((Wheel[1].GetClicked() * 20) + 10);
                foreach (var data in Wheel)
                {
                    if (data.Value.Clicked == false)
                    {
                        Wheel[data.Key].Value = rand.Next(0, 6);
                        Wheel[data.Key].ButtonRef.Source = Images[Wheel[data.Key].Value];
                    }
                }
                Calculate();

                for (int i = 1; i <= 3; i++)
                {
                    if (Wheel[i].Clicked)
                    {
                        Wheel[i].RemoveClick();
                        Wheel[i].ButtonRef.Opacity = 1;
                    }
                    
                }
                return $"${money}";
            }
            else
            {
                return "Not Enough Money!";
            }


            
        }

        void Calculate()
        {
            int total = 0;
            for (int i = 1; i <= 3; i++)
            {
                total += BinaryNum[(Wheel[i].Value)];
            }


            if (Wheel[1].Value == 0 || Wheel[2].Value == 0 || Wheel[3].Value == 0)
            {
                int loseTotal = 0;
                for (int x = 1; x <= 3; x++)
                {
                    if (Wheel[x].Value == 0)
                    {
                        loseTotal++;
                    }
                }
                if (money - (loseTotal * 20) >= 0)
                {
                    money -= (loseTotal * 20);
                    string OutputText = $"You Lose ${loseTotal * 20}";
                    Textinfo.Text = OutputText;
                    Speak(OutputText);
                }
                else
                {
                    money = 0;
                }
            }
            else
            {
                if (PayoutData.ContainsKey(total))
                {
                    money += PayoutData[total].Amount;
                    string OutputText = $"You Win ${PayoutData[total].Amount}!";
                    Textinfo.Text = OutputText;
                    Speak(OutputText);
                }
            }
        }

        public string Hold(int key)
        {

            if (Wheel[key].Clicked)
            {
                Wheel[key].RemoveClick();
                Wheel[key].ButtonRef.Opacity = 1;
                
            }
            else
            {
                if (Wheel[key].GetClicked() + 1 <= 2)
                {
                    if (money - (((Wheel[key].GetClicked()+1) * 20) + 10) >= 0)
                    {
                        Wheel[key].Setclick();
                        Wheel[key].ButtonRef.Opacity = 0.2;
                    }
                    else
                    {
                        return "Not Enough Money!";
                    }
                }
                else
                {
                    return "You can only Hold Max of 2";
                }
            }
            return $"${money}";
        }
    }

    class Payout
    {
        public int Amount;

        public Payout(int amount)
        {
            Amount = amount;
        }
    }
    class DataClass
    {
        public static int AmountClicked = 0;
        public bool Clicked;
        public int Value;
        public Image ButtonRef;
        public DataClass()
        {
            Clicked = false;
            Value = 0;
        }

        public void Setclick()
        {
            Clicked = true;
            AmountClicked++;
        }
        public void RemoveClick()
        {
            Clicked = false;
            AmountClicked--;
        }

        public int GetClicked()
        {
            return AmountClicked;
        }
    }
}
