using OpenCvSharp;
using OpenCvSharp.Extensions;
using Point = OpenCvSharp.Point;
using Size = OpenCvSharp.Size;

namespace ClickMashine
{   
    internal class MatControl
    {
        private Mat mat { get; set; }
        private List<Mat> elements = new List<Mat>();
        private List<Mat> images = new List<Mat>();
        private List<List<Point>> listElements = new List<List<Point>>();
        public MatControl(Mat mat)
        {
            this.mat = mat;
        }
        public void SaveDate(string path, string name)
        {
            if (images.Count < 4)
                using (var srWindow = new Window("super resolution"))
                {
                    for (int i = 0; i < images.Count; i++)
                    {
                        Mat image = images[i];
                        srWindow.ShowImage(image);
                        Cv2.WaitKey(1);
                        image.SaveImage(path + name[i] + "/" + new DirectoryInfo(path + name[i]).GetFiles().Length.ToString() + ".png");
                    }
                }
        }
        public bool SplitImage(int imgX, int imgY, int range = 5, int Cout = 3)
        {
            bool bImage = false;
            Point startImage = new Point(0, 0);
            Point endImage = new Point(0, 0);
            for (int x = 0; x < mat.Width; x++)
            {
                bool z = false;
                for (int y = 0; y < mat.Height; y++)
                {
                    byte color = mat.At<byte>(y, x);
                    if (color == 255)
                    {
                        z = true;
                        break;
                    }
                }
                if (z)
                {
                    if (!bImage)
                    {
                        startImage.X = x;
                        bImage = true;
                    }
                }
                else
                {
                    if (bImage)
                    {
                        endImage.X = x;
                        bool B = false;
                        for (int j = 0; j < mat.Height; j++)
                        {
                            bool m = false;
                            for (int i = startImage.X; i < endImage.X; i++)
                            {
                                byte color = mat.At<byte>(j, i);
                                if (color == 255)
                                {
                                    m = true;
                                    break;
                                }
                            }
                            if (j == mat.Height - 1)
                                m = false;
                            if (m)
                            {
                                if (!B)
                                {
                                    startImage.Y = j;
                                    B = true;
                                }
                            }
                            else
                            {
                                if (B)
                                {
                                    endImage.Y = j;
                                    if (endImage.X - startImage.X >= imgX - range && endImage.Y - startImage.Y >= imgY - range)
                                        if (endImage.X - startImage.X <= imgX + range && endImage.Y - startImage.Y <= imgX + range)
                                        {
                                            Mat img = mat.Clone(new Rect(startImage.X, startImage.Y, endImage.X - startImage.X, endImage.Y - startImage.Y));
                                            images.Add(img.Resize(new Size(imgX, imgY)));
                                        }
                                    B = false;
                                }
                            }
                        }
                        bImage = false;
                    }
                }
            }
            if (images.Count == Cout)
                return true;
            return false;
        }
        public int FindElements()
        {
            for (int x = 0; x < mat.Width; x++)
            {
                for (int y = 0; y < mat.Height; y++)
                {
                    byte color = mat.At<byte>(y, x);
                    if (color == 255)
                    {
                        List<Point> points = new List<Point>();
                        CheckPoinAround(new Point(x, y), ref points);
                        listElements.Add(points);
                        int maxX = 0;
                        foreach (Point p in points)
                            if (p.X > maxX)
                                maxX = p.X;
                        x = maxX + 1;
                        break;
                    }
                }
            }
            foreach(List<Point> ps in listElements)
            {
                Point min = new Point(mat.Width, mat.Height);
                Point max = new Point(0,0);
                foreach(Point p in ps)
                {
                    if (p.X < min.X)
                        min.X = p.X;
                    else if (p.X > max.X)
                        max.X = p.X;
                    if(p.Y < min.Y)
                        min.Y = p.Y;
                    else if(p.Y > max.Y)
                        max.Y = p.Y;
                }
                elements.Add(mat.Clone(new Rect(min.X, min.Y, max.X - min.X + 1, max.Y - min.Y + 1)));
            }
            return listElements.Count;
        }
        public int ElementsToImages(Size sizeImage)
        {
            foreach(Mat mat in elements)
                images.Add(mat.Resize(sizeImage));
            return images.Count;
        }
        private void CheckPoinAround(Point point, ref List<Point> rPoints)
        {           
            rPoints.Add(point);
            if (point.Y > 0)
                NewPointCheckAround(new Point(point.X, point.Y - 1), ref rPoints);
            if (point.Y > 0 && point.X < mat.Width)
                NewPointCheckAround(new Point(point.X + 1, point.Y - 1), ref rPoints);
            if (point.X < mat.Width)
                NewPointCheckAround(new Point(point.X + 1, point.Y), ref rPoints);
            if (point.X < mat.Width && point.Y < mat.Height)
                NewPointCheckAround(new Point(point.X + 1, point.Y + 1), ref rPoints);
            if (point.Y < mat.Height)
                NewPointCheckAround(new Point(point.X, point.Y + 1), ref rPoints);
            if (point.Y < mat.Height && point.X > 0)
                NewPointCheckAround(new Point(point.X - 1, point.Y + 1), ref rPoints);
            if (point.X > 0)
                NewPointCheckAround(new Point(point.X - 1, point.Y), ref rPoints);
        }
        private void NewPointCheckAround(Point newPoint, ref List<Point> rPoints)
        {
            byte color = mat.At<byte>(newPoint.Y, newPoint.X);
            if (color == 255)
                if (rPoints.IndexOf(newPoint) == -1)
                    CheckPoinAround(newPoint, ref rPoints);
        }
        public void ShowImages()
        {
            using (var srWindow = new Window("super resolution"))
            {
                foreach (var image in images)
                {
                    srWindow.ShowImage(image);
                    Cv2.WaitKey();
                }
            }
        }
        public string Predict(NN nn)
        {
            string predict = "";
            foreach(var image in images)
                predict += nn.Predict(image).Num.ToString();
            return predict;
        }
    }
    abstract class ImageControl
    {
        public NN nn;
        //protected Size sizeImage;
        //public void SaveDate(string path, string name)
        //{
        //    if (images.Count != 0)
        //        using (var srWindow = new Window("super resolution"))
        //        {
        //            for (int i = 0; i < images.Count; i++)
        //            {
        //                Mat image = images[i];
        //                srWindow.ShowImage(image);
        //                Cv2.WaitKey(1);
        //                image.SaveImage(path + name[i] + "/" + new DirectoryInfo(path + name[i]).GetFiles().Length.ToString() + ".png");
        //            }
        //        }
        //}
        protected int FindElements(Mat mat, out List<Mat> elements)
        {
            elements = new List<Mat>();
            List<List<Point>> listElementsPoint = new List<List<Point>>();
            for (int x = 0; x < mat.Width; x++)
            {
                for (int y = 0; y < mat.Height; y++)
                {
                    byte color = mat.At<byte>(y, x);
                    if (color == 255)
                    {
                        List<Point> points = new List<Point>();
                        CheckPoinAround(ref mat, new Point(x, y), ref points);
                        listElementsPoint.Add(points);
                        int maxX = 0;
                        foreach (Point p in points)
                            if (p.X > maxX)
                                maxX = p.X;
                        x = maxX + 1;
                        break;
                    }
                }
            }
            foreach (List<Point> ps in listElementsPoint)
            {
                Point min = new Point(mat.Width, mat.Height);
                Point max = new Point(0, 0);
                foreach (Point p in ps)
                {
                    if (p.X < min.X)
                        min.X = p.X;
                    else if (p.X > max.X)
                        max.X = p.X;
                    if (p.Y < min.Y)
                        min.Y = p.Y;
                    else if (p.Y > max.Y)
                        max.Y = p.Y;
                }
                elements.Add(mat.Clone(new Rect(min.X, min.Y, max.X - min.X + 1, max.Y - min.Y + 1)));
            }
            return elements.Count;
        }
        protected virtual int ElementsToImages(Size sizeImage, List<Mat> elements, out List<Mat> images)
        {
            images = new List<Mat>();
            foreach (Mat mat in elements)
                images.Add(mat.Resize(sizeImage));
            return images.Count;
        }
        private void CheckPoinAround(ref Mat mat, Point point, ref List<Point> rPoints)
        {
            rPoints.Add(point);
            if (point.Y > 0)
                NewPointCheckAround(ref mat, new Point(point.X, point.Y - 1), ref rPoints);
            if (point.Y > 0 && point.X < mat.Width)
                NewPointCheckAround(ref mat, new Point(point.X + 1, point.Y - 1), ref rPoints);
            if (point.X < mat.Width)
                NewPointCheckAround(ref mat, new Point(point.X + 1, point.Y), ref rPoints);
            if (point.X < mat.Width && point.Y < mat.Height)
                NewPointCheckAround(ref mat, new Point(point.X + 1, point.Y + 1), ref rPoints);
            if (point.Y < mat.Height)
                NewPointCheckAround(ref mat, new Point(point.X, point.Y + 1), ref rPoints);
            if (point.Y < mat.Height && point.X > 0)
                NewPointCheckAround(ref mat, new Point(point.X - 1, point.Y + 1), ref rPoints);
            if (point.X > 0)
                NewPointCheckAround(ref mat, new Point(point.X - 1, point.Y), ref rPoints);
        }
        private void NewPointCheckAround(ref Mat mat, Point newPoint, ref List<Point> rPoints)
        {
            byte color = mat.At<byte>(newPoint.Y, newPoint.X);
            if (color == 255)
                if (rPoints.IndexOf(newPoint) == -1)
                    CheckPoinAround(ref mat, newPoint, ref rPoints);
        }
        public void ShowImages(Mat[] images)
        {
            using (var srWindow = new Window("super resolution"))
            {
                foreach (var image in images)
                {
                    srWindow.ShowImage(image);
                    Cv2.WaitKey();
                }
            }
        }
        protected virtual Mat ImageNormalize(Bitmap bitmap)
        {
            Mat mat = BitmapConverter.ToMat(bitmap);
            Cv2.Split(mat, out Mat[] mat_channels1);
            mat = mat_channels1[0];
            Cv2.Threshold(mat, mat, 140, 255, ThresholdTypes.BinaryInv);
            return mat;
        }
        protected virtual List<Mat> BitmapToPredict(Bitmap bitmap)
        {
            FindElements(ImageNormalize(bitmap), out List<Mat> elements);
            ElementsToImages(nn.Size,elements,out List<Mat> images);
            return images;
        }
        public virtual string Predict(Bitmap bitmap)
        {
            string predict = "";
            foreach (var image in BitmapToPredict(bitmap))
                predict += nn.Predict(image).Num.ToString();
            return predict;
        }
    }
    class ImageControlWmrClick : ImageControl
    {
        private int CheckImageCompareColor(Mat bitmap)
        {
            for (int i = 0; i < bitmap.Cols; i++)
            {
                for (int j = 0; j < bitmap.Rows; j++)
                {
                    Vec3b color = bitmap.At<Vec3b>(j, i);
                    if (color.Item0 > 205 && color.Item1 == 0 && color.Item2 == 0)
                    {
                        return 2;
                    }
                    else if (color.Item0 == 0 && color.Item1 > 205 && color.Item2 == 0)
                    {
                        return 1;
                    }
                    else if (color.Item0 == 0 && color.Item1 == 0 && color.Item2 > 205)
                    {
                        return 0;
                    }
                }
            }
            return -1;
        }
        private Mat WmrFastClickGray(Mat mat)
        {
            int type = CheckImageCompareColor(mat);
            Mat gray = new Mat();
            gray.Create(mat.Size(), MatType.CV_8UC1);
            for (int i = 0; i < mat.Rows; i++)
            {
                for (int j = 0; j < mat.Cols; j++)
                {
                    Vec3b color = mat.At<Vec3b>(i, j);
                    if (type == 0)
                    {
                        if (color.Item0 == 0 && color.Item1 == 0 && color.Item2 > 205)
                            gray.At<byte>(i, j) = 255;
                        else gray.At<byte>(i, j) = 0;
                    }
                    else if (type == 1)
                    {
                        if (color.Item0 == 0 && color.Item1 > 205 && color.Item2 == 0) gray.At<int>(i, j) = 255;
                        else gray.At<byte>(i, j) = 0;
                    }
                    else if (type == 2)
                    {
                        if (color.Item0 > 205 && color.Item1 == 0 && color.Item2 == 0) gray.At<int>(i, j) = 255;
                        else gray.At<byte>(i, j) = 0;
                    }
                }
            }
            return gray;
        }
        public ImageControlWmrClick(string pathNN)
        {
            nn = new WmrFastNNClick(pathNN);
        }
        protected override int ElementsToImages(Size sizeImage, List<Mat> elements, out List<Mat> images)
        {
            images = new List<Mat>();
            foreach (Mat mat in elements)
                images.Add(mat.Resize(sizeImage));
            return images.Count;
        }
        protected override Mat ImageNormalize(Bitmap bitmap)
        {
            return WmrFastClickGray(BitmapConverter.ToMat(bitmap));
        }
    }
    class ImageControlWmrAuth : ImageControl
    {
        public ImageControlWmrAuth(string pathNN) : base()
        {
            nn = new WmrFastNNAuth(pathNN);
        }
        protected override int ElementsToImages(Size sizeImage, List<Mat> elements, out List<Mat> images)
        {
            images = new List<Mat>();
            foreach (Mat mat in elements)
                images.Add(mat.Resize(sizeImage));
            return images.Count;
        }

        protected override Mat ImageNormalize(Bitmap bitmap)
        {
            Mat mat = BitmapConverter.ToMat(bitmap);
            Cv2.Split(mat, out Mat[] mat_channels1);
            mat = mat_channels1[0];
            Cv2.Threshold(mat, mat, 140, 255, ThresholdTypes.BinaryInv);
            return mat;
        }
    }
    class VipClickImageConrol : ImageControl {
        public VipClickImageConrol(string pathNN) : base()
        {
            nn = new VipClickNN(pathNN);
        }
        protected override int ElementsToImages(Size sizeImage, List<Mat> elements, out List<Mat> images)
        {
            images = new List<Mat>();
            foreach (Mat mat in elements)
                images.Add(mat.Resize(sizeImage));
            return images.Count;
        }
        protected override Mat ImageNormalize(Bitmap bitmap)
        {
            Mat mat = BitmapConverter.ToMat(bitmap);
            Cv2.CvtColor(mat, mat, ColorConversionCodes.BGR2GRAY);
            Cv2.Threshold(mat, mat, 140, 255, ThresholdTypes.Binary);
            return mat;
        }
        public override string Predict(Bitmap bitmap)
        {
            FindElements(ImageNormalize(bitmap), out List<Mat> elements);
            if (elements.Count > 0)
            {
                Console.WriteLine(elements[0].Cols);
                if (13 <= elements[0].Cols && elements[0].Cols <= 15)
                    return elements.Count.ToString();
                else if (elements.Count == 3)
                {
                    //ElementsToImages(nn.Size, new List<Mat>(new[] { elements[0], elements[2] }), out List<Mat> images);
                    if(elements[1].Rows > 5)
                        return (nn.Predict(elements[0]).Num + nn.Predict(elements[2]).Num + 1).ToString();
                    else
                        return (nn.Predict(elements[0]).Num - nn.Predict(elements[2]).Num + 1).ToString();
                }
            }
            return "";
        }
        public void GetTrainingSet(string path)
        {

        }
    }
}
