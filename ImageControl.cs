using OpenCvSharp;
using OpenCvSharp.Extensions;
using Point = OpenCvSharp.Point;
using Size = OpenCvSharp.Size;

namespace ClickMashine
{
    // delegate bool CompareColor(Color color1);
    //internal class MatrixImage
    //{
    //    public bool[] arr;
    //    public int rows;
    //    public int cols;
    //    public MatrixImage(int rows, int cols)
    //    {
    //        this.cols = cols;
    //        this.rows = rows;
    //        arr = new bool[rows * cols];
    //    }
    //    public MatrixImage(Bitmap bitmap, CompareColor compare)
    //    {
    //        this.cols=bitmap.Width;
    //        this.rows=bitmap.Height;
    //        arr = new bool[rows * cols];
    //        for (int i = 0; i < rows; i++)
    //        {
    //            for(int j = 0; j < cols; j++)
    //            {
    //                Color c = bitmap.GetPixel(j, i);
    //                if (compare(c))
    //                    arr[i * cols + j] = true;
    //                else
    //                    arr[i * cols + j] = false;
    //            }
    //        }
    //    }
    //    public bool this[int row,int col]
    //    {
    //        get
    //        {
    //            return arr[row * cols + col];
    //        }

    //        set
    //        {
    //            arr[row * cols + col] = value;
    //        }
    //    }
    //    public void Calibrate(int row, int col)
    //    {
    //        if (row > rows)
    //        {
    //            int difference = row - rows;
    //            if (difference % 2 == 1) //нечетное
    //            {
    //                AddRow(rows / 2);
    //                difference--;
    //            }
    //            if(difference != 0)
    //            {
    //                int step = rows / difference;
    //                int start = step / 2;
    //                for(int d = 0; d < difference; d++)
    //                {
    //                    AddRow(start);
    //                    start += step + 1;
    //                }
    //            }
    //        }
    //        else
    //        {
    //            int difference = rows - row;
    //            if (difference % 2 == 1) //нечетное
    //            {
    //                DeleteRow(rows / 2);
    //                difference--;
    //            }
    //            if (difference != 0)
    //            {
    //                int step = rows / difference;
    //                int start = step / 2;
    //                for (int d = 0; d < difference; d++)
    //                {
    //                    DeleteRow(start);
    //                    start += step + 1;
    //                }
    //            }
    //        }
    //        if (col > cols)
    //        {
    //            int difference = col - cols;
    //            if (difference % 2 == 1) //нечетное
    //            {
    //                AddCol(cols / 2);
    //                difference--;
    //            }
    //            if (difference != 0)
    //            {
    //                int step = cols / difference;
    //                int start = step / 2;
    //                for (int d = 0; d < difference; d++)
    //                {
    //                    AddCol(start);
    //                    start += step;
    //                }
    //            }
    //        }
    //        else
    //        {
    //            int difference = cols - col;
    //            if (difference % 2 == 1) //нечетное
    //            {
    //                DeleteCol(cols / 2);
    //                difference--;
    //            }
    //            if (difference != 0)
    //            {
    //                int step = cols / difference;
    //                int start = step / 2;
    //                for (int d = 0; d < difference; d++)
    //                {
    //                    DeleteCol(start);
    //                    start += step;
    //                }
    //            }
    //        }
    //    }
    //    private void AddRow(int position)
    //    {
    //        rows++;
    //        bool [] newArr = new bool[rows * cols];
    //        for(int row = 0; row < rows; row++)
    //        {
    //            for(int col = 0; col < cols; col++)
    //            {
    //                if(position < row)
    //                    newArr[row * cols + col] = arr[(row-1) * cols + col];
    //                else
    //                    newArr[row * cols + col] = arr[row * cols + col];
    //            }
    //        }
    //        arr = newArr;
    //    }
    //    private void AddCol(int position)
    //    {
    //        cols++;
    //        bool[] newArr = new bool[rows * cols];
    //        for (int col = 0; col < cols; col++)
    //        {
    //            for (int row = 0; row < rows; row++)
    //            {
    //                if (position < col)
    //                    newArr[row * cols + col] = arr[row * (cols - 1) + (col-1)];
    //                else
    //                    newArr[row * cols + col] = arr[row * (cols - 1) + col];
    //            }
    //        }
    //        arr = newArr;
    //    }
    //    private void DeleteRow(int position)
    //    {
    //        rows--;
    //        bool[] newArr = new bool[rows * cols];
    //        for (int row = 0; row < rows; row++)
    //        {
    //            for (int col = 0; col < cols; col++)
    //            {
    //                if (position < row)
    //                    newArr[row * cols + col] = arr[(row + 1) * cols + col];
    //                else
    //                    newArr[row * cols + col] = arr[row * cols + col];
    //            }
    //        }
    //        arr = newArr;
    //    }
    //    private void DeleteCol(int position)
    //    {
    //        cols--;
    //        bool[] newArr = new bool[rows * cols];
    //        for (int col = 0; col < cols; col++)
    //        {
    //            for (int row = 0; row < rows; row++)
    //            {
    //                if (position < col)
    //                    newArr[row * cols + col] = arr[row * (cols + 1) + (col + 1)];
    //                else
    //                    newArr[row * cols + col] = arr[row * (cols + 1) + col];
    //            }
    //        }
    //        arr = newArr;
    //    }
    //    public override string ToString()
    //    {
    //        string s = "";
    //        for (int i = 0; i < rows; ++i)
    //        {
    //            for (int j = 0; j < cols; ++j)
    //                if (arr[i * cols + j] == true)
    //                    s += "1";
    //                else
    //                    s += "0";
    //            s += Environment.NewLine;
    //        }
    //        return s;
    //    }
    //    public ModelInput GetModelInput(int row, int col)
    //    {
    //        Calibrate(row, col);
    //        ModelInput modelInput = new ModelInput();
    //        modelInput.PixelValues = new float[rows * cols];
    //        for (int i = 0; arr.Length > i; i++)
    //        {
    //            if (arr[i] == true)
    //                modelInput.PixelValues[i] = 1;
    //            else
    //                modelInput.PixelValues[i] = 0;
    //        }
    //        return modelInput;
    //    }
    //    public string GetStringDate()
    //    {
    //        string str = "";
    //        for (int i = 0; arr.Length > i; i++)
    //        {
    //            if (arr[i] == true)
    //                str += "1";
    //            else
    //                str += "0";
    //            str += ",";
    //        }
    //        return str;
    //    }
    //    public void SaveToDateFile(string path, char number)
    //    {
    //        using (StreamWriter writer = new StreamWriter(path, true))
    //        {
    //            writer.WriteLine(GetStringDate() + number);
    //        }
    //    }
    //}
    //internal class ImageControl
    //{
    //    public Bitmap bitmap { get; set; }
    //    public List<Bitmap> images = new List<Bitmap>();
    //    public ImageControl(Bitmap bitmap)
    //    {
    //        this.bitmap = bitmap;
    //    }
    //    public void SplitImage(CompareColor compareColor, int minX = 5, int minY = 5)
    //    {
    //        bool bImage = false;
    //        Point startImage = new Point(0, 0);
    //        Point endImage = new Point(0, 0);
    //        for (int x = 0; x < bitmap.Width; x++)
    //        {
    //            bool z = false;
    //            for (int y = 0; y < bitmap.Height; y++)
    //            {
    //                Color color = bitmap.GetPixel(x, y);
    //                if (compareColor(color))
    //                {
    //                    z = true;
    //                    break;
    //                }
    //            }
    //            if (z)
    //            {
    //                if (!bImage)
    //                {
    //                    startImage.X = x;
    //                    bImage = true;
    //                }
    //            }
    //            else
    //            {
    //                if (bImage)
    //                {
    //                    endImage.X = x;
    //                    bool B = false;
    //                    for (int j = 0; j < bitmap.Height; j++)
    //                    {
    //                        bool m = false;
    //                        for (int i = startImage.X; i < endImage.X; i++)
    //                        {
    //                            Color color = bitmap.GetPixel(i, j);
    //                            if (compareColor(color))
    //                            {
    //                                m = true;
    //                                break;
    //                            }
    //                        }
    //                        if(j == bitmap.Height - 1)
    //                            m = false;
    //                        if (m)
    //                        {
    //                            if (!B)
    //                            {
    //                                startImage.Y = j;
    //                                B = true;
    //                            }
    //                        }
    //                        else
    //                        {
    //                            if (B)
    //                            {
    //                                endImage.Y = j;
    //                                if (endImage.X - startImage.X > minX && endImage.Y - startImage.Y > minY)
    //                                    images.Add(bitmap.Clone(new Rectangle(startImage.X, startImage.Y, endImage.X - startImage.X, endImage.Y - startImage.Y), bitmap.PixelFormat));
    //                                B = false;
    //                            }
    //                        }
    //                    }
    //                    bImage = false;
    //                }
    //            }
    //        }
    //    }
    //}

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
        protected Size sizeImage;
        public ImageControl(Size size, string pathNN)
        {
            this.sizeImage = new Size(size.Width, size.Height);
            nn = new WmrFastNNAuth(sizeImage, pathNN);
        }
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
        private int FindElements(Mat mat, out List<Mat> elements)
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
        public virtual int ElementsToImages(Size sizeImage, List<Mat> elements, out List<Mat> images)
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
        //public void ShowImages()
        //{
        //    using (var srWindow = new Window("super resolution"))
        //    {
        //        foreach (var image in images)
        //        {
        //            srWindow.ShowImage(image);
        //            Cv2.WaitKey();
        //        }
        //    }
        //}
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
            ElementsToImages(sizeImage,elements,out List<Mat> images);
            return images;
        }
        public string Predict(Bitmap bitmap)
        {
            string predict = "";
            foreach (var image in BitmapToPredict(bitmap))
                predict += nn.Predict(image).Num.ToString();
            return predict;
        }
    }

    class ImageConrolWmrClick : ImageControl
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
        public ImageConrolWmrClick(Size imgSize, string pathNN) : base(imgSize, pathNN)
        {
        }
        public override int ElementsToImages(Size sizeImage, List<Mat> elements, out List<Mat> images)
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
        public ImageControlWmrAuth(Size imgSize, string pathNN) : base(imgSize, pathNN)
        {
        }
        public override int ElementsToImages(Size sizeImage, List<Mat> elements, out List<Mat> images)
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
        public VipClickImageConrol(Size imgSize, string pathNN) : base(imgSize, pathNN)
        {
        }
        public override int ElementsToImages(Size sizeImage, List<Mat> elements, out List<Mat> images)
        {
            images = new List<Mat>();
            foreach (Mat mat in elements)
                images.Add(mat.Resize(sizeImage));
            return images.Count;
        }
        protected override Mat ImageNormalize(Bitmap bitmap)
        {
            Mat mat = BitmapConverter.ToMat(bitmap);
            Cv2.Threshold(mat, mat, 140, 255, ThresholdTypes.BinaryInv);
            return mat;
        }
    }
}
