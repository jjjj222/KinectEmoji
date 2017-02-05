using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KinectEmoji
{
    class MyPoint
    {
        public double x { get; set; }
        public double y { get; set; }
        public double z { get; set; }

        public MyPoint(double x = 0, double y = 0, double z = 0)
        {
            setValue(x, y, z);
        }

        public void setValue(double ix, double iy, double iz)
        {
            x = ix; y = iy; z = iz;
        }

        public double distance(MyPoint rhs)
        {
            return Math.Sqrt(
                (x - rhs.x) * (x - rhs.x) +
                (y - rhs.y) * (y - rhs.y) +
                (z - rhs.z) * (z - rhs.z)
            );
        }

        public String json()
        {
            String str = "{";
            str += String.Format("x:\"{0}\",", x);
            str += String.Format("y:\"{0}\",", y);
            str += String.Format("z:\"{0}\"", z);
            str += "}";
            return str;
        }

        public override String ToString()
        {
            return String.Format("({0:F2}, {1:F2}, {2:F2})", x, y, z);
        }
    }

    class MyVector
    {
        public MyPoint start { get; }
        public MyPoint end { get; }

        public MyVector(MyPoint s, MyPoint e)
        {
            start = s; end = e;
        }

        public override String ToString()
        {
            return String.Format("({0} -> {1})", start.ToString(), end.ToString());
        }
    }

    class FaceHD
    {
        public const int EyeLeft = 0;
        public const int LefteyeInnercorner = 210;
        public const int LefteyeOutercorner = 469;
        public const int LefteyeMidtop = 241;
        public const int LefteyeMidbottom = 1104;
        public const int RighteyeInnercorner = 843;
        public const int RighteyeOutercorner = 1117;
        public const int RighteyeMidtop = 731;
        public const int RighteyeMidbottom = 1090;
        public const int LefteyebrowInner = 346;
        public const int LefteyebrowOuter = 140;
        public const int LefteyebrowCenter = 222;
        public const int RighteyebrowInner = 803;
        public const int RighteyebrowOuter = 758;
        public const int RighteyebrowCenter = 849;
        public const int MouthLeftcorner = 91;
        public const int MouthRightcorner = 687;
        public const int MouthUpperlipMidtop = 19;
        public const int MouthUpperlipMidbottom = 1072;
        public const int MouthLowerlipMidtop = 10;
        public const int MouthLowerlipMidbottom = 8;
        public const int NoseTip = 18;
        public const int NoseBottom = 14;
        public const int NoseBottomleft = 156;
        public const int NoseBottomright = 783;
        public const int NoseTop = 24;
        public const int NoseTopleft = 151;
        public const int NoseTopright = 772;
        public const int ForeheadCenter = 28;
        public const int LeftcheekCenter = 412;
        public const int RightcheekCenter = 933;
        public const int Leftcheekbone = 458;
        public const int Rightcheekbone = 674;
        public const int ChinCenter = 4;
        public const int LowerjawLeftend = 1307;
        public const int LowerjawRightend = 1327;

        public static readonly int [] MouthPoints = {MouthLeftcorner, MouthRightcorner, MouthUpperlipMidbottom, MouthLowerlipMidtop};
        public static readonly int[] LeftEyePoints = { LefteyeInnercorner, LefteyeOutercorner, LefteyeMidtop, LefteyeMidbottom };
        public static readonly int[] RightEyePoints = { RighteyeInnercorner, RighteyeOutercorner, RighteyeMidtop, RighteyeMidbottom };   
        public static readonly int[] TargetPoints = {
            MouthLeftcorner, MouthRightcorner, MouthUpperlipMidbottom, MouthLowerlipMidtop
            //LefteyeInnercorner, LefteyeOutercorner, LefteyeMidtop, LefteyeMidbottom,
            //RighteyeInnercorner, RighteyeOutercorner, RighteyeMidtop, RighteyeMidbottom
        };
        public static readonly string[] TargetPointsName = {
            "MouthLeftcorner", "MouthRightcorner", "MouthUpperlipMidbottom", "MouthLowerlipMidtop"
            //"LefteyeInnercorner", "LefteyeOutercorner", "LefteyeMidtop", "LefteyeMidbottom",
            //"RighteyeInnercorner", "RighteyeOutercorner", "RighteyeMidtop", "RighteyeMidbottom"
        };
        public static Dictionary<int, int> IndexToPos = null;

        //public MyPoint pMouthLeftcorner = new MyPoint();

        public DateTime time { get; }
        public List<MyPoint> _trackedPoints = new List<MyPoint>();

        static FaceHD()
        {
            IndexToPos = new Dictionary<int, int>();
            for (int i = 0; i < TargetPoints.Length; ++i)
            {
                IndexToPos.Add(TargetPoints[i], i);
            } 
        }

        public FaceHD()
        {
            time = DateTime.Now;
            for (int i = 0; i < TargetPoints.Length; ++i)
            {
                _trackedPoints.Add(new MyPoint());
            }
        }

        public static bool isMouthPoint(int i) { return Array.Exists(MouthPoints, element => element == i); }
        public static bool isLeftEyePoint(int i) { return Array.Exists(LeftEyePoints, element => element == i); }
        public static bool isRightEyePoint(int i) { return Array.Exists(RightEyePoints, element => element == i); }

        public void addData(int i, double x, double y, double z)
        {
            int value;
            if (IndexToPos.TryGetValue(i, out value))
            {
                _trackedPoints[value].setValue(x, y, z);
            }
        }

        public MyPoint getPoint(int index)
        {
            return _trackedPoints[IndexToPos[index]];
        }

        public double distance(int a, int b)
        {
            double result = getPoint(a).distance(getPoint(b));
            return result;
        }

        public double unitLength()
        {
            //return MyVector(getPoint(LefteyeInnercorner), getPoint(RighteyeInnercorner));
            return 0;
        }

        public String json()
        {
            String str = "{";

            for (int i = 0; i < TargetPoints.Length; ++i)
            {
                str += String.Format("{0}:{1},", TargetPointsName[i], _trackedPoints[i].json());
            }
            str += String.Format("time:\"{0}\"", time.Ticks);
            str += "}";
            return str;
        }

        public String dump_str()
        {
            
            String str = "";
            
            for (int i = 0; i < TargetPoints.Length; ++i)
            {
                str += String.Format("{0}({1}): {2}\n", TargetPointsName[i], TargetPoints[i], _trackedPoints[i].ToString());
            }
            str += String.Format("feature_mouthRatio(): {0}\n", feature_mouthRatio());

            //return String.Format("MouthLeftcorner: {0}", pMouthLeftcorner.ToString());
            /*
            MyVector v = new MyVector(_trackedPoints[0].point, _trackedPoints[1].point);
            tmp += String.Format("{0}\n", v.ToString());
            */
            //tmp += String.Format("classify_leftEyeClose: {0}\n", classify_leftEyeClose());
            //tmp += String.Format("feature_leftEyeRatio(): {0}\n", feature_leftEyeRatio());
            //tmp += String.Format("d1: {0}\n", getPoint(LefteyeMidtop).distance(getPoint(LefteyeMidbottom)));
            //tmp += String.Format("d2: {0}\n", getPoint(LefteyeOutercorner).distance(getPoint(LefteyeInnercorner)));




            return str;
        }

        public double feature_mouthRatio()
        {
            double result = distance(MouthLowerlipMidtop, MouthUpperlipMidbottom)
                / distance(MouthLeftcorner, MouthRightcorner);
            return result;
        }
    }
}