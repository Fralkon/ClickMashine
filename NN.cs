using OpenCvSharp;
using OpenCvSharp.Extensions;
using Tensorflow;
using Tensorflow.Keras.ArgsDefinition;
using Tensorflow.Keras.Engine;
using Tensorflow.Keras.Layers;
using Tensorflow.NumPy;
using static Tensorflow.Binding;
using static Tensorflow.KerasApi;
using Size = OpenCvSharp.Size;

namespace ClickMashine
{
    class PredictNN
    {
        public PredictNN(Tensor tensor)
        {
            Tensor = tensor;
            float[] end = Tensor.ToArray<float>();
            Num = 0;
            for (int i = 1; i < end.Length; i++)
            {
                if (end[Num] < end[i])
                    Num = i;
            }
        }
        public Tensor Tensor { get; private set; }
        public int Num { get; set; }
    }
    abstract class NN
    {
        public Size Size { get; set; }
        protected Shape imgDim;
        protected IDatasetV2 train_ds, val_ds;
        protected Model model;
        public NN(Size size, int depth)
        {
            Size = size;
            imgDim = new Shape(Size.Height, Size.Width, depth);
            tf.enable_eager_execution();
            BuildModel();
        }
        public NN(Size size, int depth, string path)
        {
            Size = size;
            imgDim = new Shape(Size.Height, Size.Width, depth);
            tf.enable_eager_execution();
            BuildModel();
            model.load_weights(path);
        }
        public NN(Size size)
        {
            Size = size;
            imgDim = new Shape(Size.Height, Size.Width, 1);
            tf.enable_eager_execution();
            BuildModel();
        }
        public NN(Size imgSize, string path)
        {
            Size = imgSize;
            imgDim = (imgSize.Height, imgSize.Width, 1);
            tf.enable_eager_execution();
            BuildModel();
            model.load_weights(path);
        }
        public virtual PredictNN Predict(Mat mat)
        {
            var arr = ConvertToPredict(mat);
            return new PredictNN(model.predict(tf.expand_dims(arr, 0)));
        }
        public virtual PredictNN Predict(Bitmap bitmap)
        {
            Mat mat = BitmapConverter.ToMat(bitmap);
            Cv2.CvtColor(mat, mat, ColorConversionCodes.BGR2RGB);
            if (mat.Cols != imgDim[0] || mat.Rows != imgDim[1])
                mat = mat.Resize(new Size(imgDim[0], imgDim[1]));
            int rows = mat.Rows;
            int cols = mat.Cols;
            byte[] arr = new byte[rows * cols * 3];
            int p = 0;
            for (int i = 0; i < mat.Rows; i++)
            {
                for (int j = 0; j < mat.Cols; j++)
                {
                    Vec3b color = mat.At<Vec3b>(i, j);
                    arr[p++] = color.Item0;
                    arr[p++] = color.Item1;
                    arr[p++] = color.Item2;
                }
            }
            NDArray numpy_array = np.array(arr);
            numpy_array = numpy_array.reshape(new int[] { rows, cols, 3 });
            return new PredictNN(model.predict(tf.expand_dims(numpy_array, 0)));
        }
        protected NDArray ConvertToPredict(Mat mat)
        {
            mat = mat.Resize(new Size(imgDim[1], imgDim[0]));
            mat.ConvertTo(mat, MatType.CV_32SC1);
            mat.GetArray(out int[] arr);
            var numpy_array = np.array(arr);
            return numpy_array.reshape(new int[] { mat.Rows, mat.Cols, 1 });
        }
        public void Save(string path)
        {
            model.save_weights(path);
        }
        public void Load(string path)
        {
            model.load_weights(path);
        }
        protected abstract void BuildModel();
        public void Train(string path, int epochs)
        {
            PrepareData(path);
            model.fit(train_ds, epochs: epochs);
        }
        protected void PrepareData(string pathData)
        {
            train_ds = keras.preprocessing.image_dataset_from_directory(pathData,
                validation_split: 0.2f,
                color_mode:"rgb",
                subset: "training",
                seed: 123,
                image_size: (imgDim[0], imgDim[1]));

            val_ds = keras.preprocessing.image_dataset_from_directory(pathData,
            validation_split: 0.2f,
            subset: "validation",
            color_mode: "rgb",
            seed: 123,
            image_size: (imgDim[0], imgDim[1]));

            train_ds = train_ds.shuffle(1000).prefetch(buffer_size: -1);
            val_ds = val_ds.prefetch(buffer_size: -1);
            
        }
    }
    class WmrFastNNClick : NN
    {
        public WmrFastNNClick(string path) : base(new Size(20, 26),path)
        {
        }
        public override PredictNN Predict(Mat mat)
        {
            var arr = ConvertToPredict(mat);
            return new PredictNN(model.predict(tf.expand_dims(arr, 0)));
        }
        protected override void BuildModel()
        {
            var layers = new LayersApi();

            // input layer
            var inputs = keras.layers.Input(shape: imgDim, name: "img");

            // convolutional layer
            var x = layers.Rescaling(1.0f / 255, input_shape: imgDim).Apply(inputs);
            x = layers.Conv2D(10, 3, activation: "relu").Apply(x);
            x = layers.Conv2D(64, 3, activation: "relu").Apply(x);
            x = layers.Conv2D(64, 3, activation: "relu").Apply(x);
            x = layers.GlobalAveragePooling2D().Apply(x);
            x = layers.Dense(256, activation: "relu").Apply(x);
            x = layers.Dropout(0.5f).Apply(x);

            // output layer
            var outputs = layers.Dense(10).Apply(x);

            // build keras model
            model = (Model)keras.Model(inputs, outputs, name: "WmrClick");
            model.summary();

            model.compile(optimizer: keras.optimizers.Adam(),
                loss: keras.losses.SparseCategoricalCrossentropy(from_logits: true),
                metrics: new[] { "accuracy" });
        }
    }
    class WmrFastNNAuth : NN
    {
        public WmrFastNNAuth(string path) : base(new Size(8, 10),path)
        {

        }
        public override PredictNN Predict(Mat mat)
        {
            var arr = ConvertToPredict(mat);
            PredictNN predictNN = new PredictNN(model.predict(tf.expand_dims(arr, 0)));
            predictNN.Num++;
            return predictNN;
        }
        protected override void BuildModel()
        {
            var layers = new LayersApi();

            // input layer
            var inputs = keras.layers.Input(shape: imgDim, name: "img");

            // convolutional layer
            var x = layers.Rescaling(1.0f / 255, input_shape: imgDim).Apply(inputs);
            x = layers.Conv2D(10, 3, activation: "relu").Apply(x);
            x = layers.Conv2D(64, 3, activation: "relu").Apply(x);
            x = layers.Conv2D(64, 3, activation: "relu").Apply(x);
            x = layers.GlobalAveragePooling2D().Apply(x);
            x = layers.Dense(256, activation: "relu").Apply(x);
            x = layers.Dropout(0.5f).Apply(x);

            // output layer
            var outputs = layers.Dense(9).Apply(x);

            // build keras model
            model = (Model)keras.Model(inputs, outputs, name: "WmrAuth");
            model.summary();

            // compile keras model in tensorflow static graph
            model.compile(optimizer: keras.optimizers.Adam(),
               loss: keras.losses.SparseCategoricalCrossentropy(from_logits: true),
               metrics: new[] { "accuracy" });

        }
    }
    class VipClickNN : NN
    {
        public VipClickNN() : base(new Size(12, 18))
        {
        }
        public VipClickNN(string path) : base(new Size(12, 18), path)
        {
            Size = new Size();
        }
        public override PredictNN Predict(Mat mat)
        {
            var arr = ConvertToPredict(mat);
            return new PredictNN(model.predict(tf.expand_dims(arr, 0)));
        }
        protected override void BuildModel()
        {
            var layers = new LayersApi();

            // input layer
            var inputs = keras.layers.Input(shape: imgDim, name: "img");

            // convolutional layer
            var x = layers.Rescaling(1.0f / 255, input_shape: imgDim).Apply(inputs);
            x = layers.Conv2D(10, 3, activation: "relu").Apply(x);
            x = layers.Conv2D(64, 3, activation: "relu").Apply(x);
            x = layers.Conv2D(64, 3, activation: "relu").Apply(x);
            x = layers.GlobalAveragePooling2D().Apply(x);
            x = layers.Dense(256, activation: "relu").Apply(x);
            x = layers.Dropout(0.5f).Apply(x);

            // output layer
            var outputs = layers.Dense(9).Apply(x);

            // build keras model
            model = (Model)keras.Model(inputs, outputs, name: "VipClickNN");
            model.summary();

            // compile keras model in tensorflow static graph
            model.compile(optimizer: keras.optimizers.Adam(),
               loss: keras.losses.SparseCategoricalCrossentropy(from_logits: true),
               metrics: new[] { "accuracy" });
        }
    }
    class ProfitcentNN : NN
    {
        public ProfitcentNN() : base(new Size(75, 75), 3)
        {
        }
        public ProfitcentNN(string path) : base(new Size(75, 75), 3, path)
        {
        }
        public PredictNN Predict(Bitmap bitmap)
        {
            Mat mat = BitmapConverter.ToMat(bitmap);
            Cv2.CvtColor(mat, mat, ColorConversionCodes.BGR2RGB);
            if (mat.Cols != imgDim[0] || mat.Rows != imgDim[1])
                mat = mat.Resize(new Size(imgDim[0], imgDim[1]));
            int rows = mat.Rows;
            int cols = mat.Cols;
            byte[] arr = new byte[rows * cols * 3];
            int p = 0;
            for (int i = 0; i < mat.Rows; i++)
            {
                for (int j = 0; j < mat.Cols; j++)
                {
                    Vec3b color = mat.At<Vec3b>(i, j);
                    arr[p++] = color.Item0;
                    arr[p++] = color.Item1;
                    arr[p++] = color.Item2;
                }
            }
            NDArray numpy_array = np.array(arr);
            numpy_array = numpy_array.reshape(new int[] { rows, cols, 3 });
            PredictNN predictNN = new PredictNN(model.predict(tf.expand_dims(numpy_array, 0)));
            return predictNN;
        }
        protected override void BuildModel()
        {
            var layers = new LayersApi();

            // input layer
            var inputs = keras.layers.Input(shape: imgDim, name: "img");

            // convolutional layer
            var x = layers.Rescaling(1.0f / 255, input_shape: imgDim).Apply(inputs);
            x = layers.Conv2D(20, 3, activation: "relu").Apply(x);
            x = layers.Conv2D(76, 3, activation: "relu").Apply(x);
            x = layers.Conv2D(76, 3, activation: "relu").Apply(x);
            x = layers.GlobalAveragePooling2D().Apply(x);
            x = layers.Dense(256, activation: "relu").Apply(x);
            x = layers.Dropout(0.5f).Apply(x);

            // output layer
            var outputs = layers.Dense(39).Apply(x);

            model = (Model)keras.Model(inputs, outputs, name: "Profitcentr");
            model.summary();

            // compile keras model in tensorflow static graph
            model.compile(optimizer: keras.optimizers.Adam(),
               loss: keras.losses.SparseCategoricalCrossentropy(from_logits: true),
               metrics: new[] { "accuracy" });

        }
    }
    class SeoClubNN : NN
    {
        public SeoClubNN() : base(new Size(75, 75), 3)
        {
        }
        public SeoClubNN(string path) : base(new Size(75, 75), 3, path)
        {
        }
        public override PredictNN Predict(Bitmap bitmap)
        {
            Mat mat = BitmapConverter.ToMat(bitmap);
            Cv2.CvtColor(mat, mat, ColorConversionCodes.BGR2RGB);
            if (mat.Cols != imgDim[0] || mat.Rows != imgDim[1])
                mat = mat.Resize(new Size(imgDim[0], imgDim[1]));
            int rows = mat.Rows;
            int cols = mat.Cols;
            byte[] arr = new byte[rows * cols * 3];
            int p = 0;
            for (int i = 0; i < mat.Rows; i++)
            {
                for (int j = 0; j < mat.Cols; j++)
                {
                    Vec3b color = mat.At<Vec3b>(i, j);
                    arr[p++] = color.Item0;
                    arr[p++] = color.Item1;
                    arr[p++] = color.Item2;
                }
            }
            NDArray numpy_array = np.array(arr);
            numpy_array = numpy_array.reshape(new int[] { rows, cols, 3 });
            PredictNN predictNN = new PredictNN(model.predict(tf.expand_dims(numpy_array, 0)));
            return predictNN;
        }
        protected override void BuildModel()
        {
            var layers = new LayersApi();

            // input layer
            var inputs = keras.layers.Input(shape: imgDim, name: "img");
            var x = layers.Rescaling(1.0f / 255, input_shape: imgDim).Apply(inputs);
            x = layers.Conv2D(20, 3, activation: "relu").Apply(x);
            x = layers.Conv2D(76, 3, activation: "relu").Apply(x);
            x = layers.Conv2D(76, 3, activation: "relu").Apply(x);
            x = layers.GlobalAveragePooling2D().Apply(x);
            x = layers.Dense(256, activation: "relu").Apply(x);
            x = layers.Dropout(0.5f).Apply(x);

            // output layer
            var outputs = layers.Dense(10).Apply(x);

            model = (Model)keras.Model(inputs, outputs, name: "SeoClub");
            model.summary();

            // compile keras model in tensorflow static graph
            model.compile(optimizer: keras.optimizers.Adam(),
               loss: keras.losses.SparseCategoricalCrossentropy(from_logits: true),
               metrics: new[] { "accuracy" });
        }
    }
    class SeoFastNN : NN
    {
        public SeoFastNN() : base(new Size(75, 75), 3)
        {
        }
        public SeoFastNN(string path) : base(new Size(75, 75), 3, path)
        {
        }
        public override PredictNN Predict(Bitmap bitmap)
        {
            Mat mat = BitmapConverter.ToMat(bitmap);
            Cv2.CvtColor(mat, mat, ColorConversionCodes.BGR2RGB);
            if (mat.Cols != imgDim[0] || mat.Rows != imgDim[1])
                mat = mat.Resize(new Size(imgDim[0], imgDim[1]));
            int rows = mat.Rows;
            int cols = mat.Cols;
            byte[] arr = new byte[rows * cols * 3];
            int p = 0;
            for (int i = 0; i < mat.Rows; i++)
            {
                for (int j = 0; j < mat.Cols; j++)
                {
                    Vec3b color = mat.At<Vec3b>(i, j);
                    arr[p++] = color.Item0;
                    arr[p++] = color.Item1;
                    arr[p++] = color.Item2;
                }
            }
            NDArray numpy_array = np.array(arr);
            numpy_array = numpy_array.reshape(new int[] { rows, cols, 3 });
            PredictNN predictNN = new PredictNN(model.predict(tf.expand_dims(numpy_array, 0)));
            return predictNN;
        }
        protected override void BuildModel()
        {
            var layers = new LayersApi();

            var inputs = keras.layers.Input(shape: imgDim, name: "img");
            var x = layers.Rescaling(1.0f / 255, input_shape: imgDim).Apply(inputs);
            x = layers.Conv2D(20, 3, activation: "relu").Apply(x);
            x = layers.Conv2D(76, 3, activation: "relu").Apply(x);
            x = layers.Conv2D(76, 3, activation: "relu").Apply(x);
            x = layers.GlobalAveragePooling2D().Apply(x);
            x = layers.Dense(256, activation: "relu").Apply(x);
            x = layers.Dropout(0.5f).Apply(x);

            var outputs = layers.Dense(23).Apply(x);

            model = (Model)keras.Model(inputs, outputs, name: "SeoFast");
            model.summary();

            model.compile(optimizer: keras.optimizers.Adam(),
               loss: keras.losses.SparseCategoricalCrossentropy(from_logits: true),
               metrics: new[] { "accuracy" });
        }
    }
}
