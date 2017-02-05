using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Media.Imaging;

namespace KinectEmoji
{
    class Emoji
    {
        public static BitmapImage happy = new BitmapImage(new Uri("ms-appx:///Images/happy.png"));
        public static BitmapImage shock = new BitmapImage(new Uri("ms-appx:///Images/shock.png"));
        public static BitmapImage none = new BitmapImage(new Uri("ms-appx:///Images/none.png"));
        public static BitmapImage no = new BitmapImage(new Uri("ms-appx:///Images/no.png"));
        public static BitmapImage wink = new BitmapImage(new Uri("ms-appx:///Images/wink.png"));
        public static BitmapImage yes = new BitmapImage(new Uri("ms-appx:///Images/yes.jpg"));
        public static BitmapImage question = new BitmapImage(new Uri("ms-appx:///Images/question.png"));
        public static BitmapImage tired = new BitmapImage(new Uri("ms-appx:///Images/tired.jpg"));
        public static BitmapImage annoyed = new BitmapImage(new Uri("ms-appx:///Images/annoyed.png"));
    }
}
