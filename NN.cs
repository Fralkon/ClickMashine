using OpenCvSharp;
using Tensorflow;
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
    class NN
    {
        protected Shape img_dim;
        protected IDatasetV2? train_ds, val_ds;
        protected Model model;
        public NN(Size imgSize)
        {
            img_dim = (imgSize.Height, imgSize.Width, 1);
            tf.enable_eager_execution();
            BuildModel();
        }
        public NN(Size imgSize, string path)
        {
            img_dim = (imgSize.Height, imgSize.Width, 1);
            tf.enable_eager_execution();
            BuildModel();
            Load(path);
        }
        public virtual PredictNN Predict(Mat mat)
        {
            var arr = ConvertToPredict(mat);
            return new PredictNN(model.predict(tf.expand_dims(arr, 0)));
        }
        protected NDArray ConvertToPredict(Mat mat)
        {
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
        protected virtual void BuildModel()
        {
            //throw new NotImplementedException();
            //var layers = new LayersApi();

            //// input layer
            //var inputs = keras.layers.Input(shape: img_dim, name: "img");

            //// convolutional layer
            //var x = layers.Rescaling(1.0f / 255, input_shape: img_dim).Apply(inputs);
            //x = layers.Conv2D(10, 3, activation: "relu").Apply(x);
            //x = layers.Conv2D(64, 3, activation: "relu").Apply(x);
            //x = layers.Conv2D(64, 3, activation: "relu").Apply(x);
            //x = layers.GlobalAveragePooling2D().Apply(x);
            //x = layers.Dense(256, activation: "relu").Apply(x);
            //x = layers.Dropout(0.5f).Apply(x);

            //// output layer
            //var outputs = layers.Dense(9).Apply(x);

            //// build keras model
            //model = keras.Model(inputs, outputs, name: "toy_resnet");
            //model.summary();

            //// compile keras model in tensorflow static graph
            //model.compile(optimizer: keras.optimizers.Adam(),
            //   loss: keras.losses.SparseCategoricalCrossentropy(from_logits: true),
            //   metrics: new[] { "accuracy" });

            //LayersApi layers = new LayersApi();

            //// input layer
            //var inputs = keras.layers.Input(shape: img_dim, name: "img");

            //// convolutional layer
            //var x = layers.Rescaling(1.0f / 255, input_shape: img_dim).Apply(inputs);
            //x = layers.Conv2D(10, 3, activation: "relu").Apply(x);
            //x = layers.Conv2D(64, 3, activation: "relu").Apply(x);
            //x = layers.Conv2D(64, 3, activation: "relu").Apply(x);
            //x = layers.GlobalAveragePooling2D().Apply(x);
            //x = layers.Dense(256, activation: "relu").Apply(x);
            //x = layers.Dropout(0.5f).Apply(x);

            //// output layer
            //var outputs = layers.Dense(10).Apply(x);

            //// build keras model
            //model = keras.Model(inputs, outputs, name: "toy_resnet");
            //model.summary();

            //// compile keras model in tensorflow static graph
            //model.compile(optimizer: keras.optimizers.Adam(),
            //   loss: keras.losses.SparseCategoricalCrossentropy(from_logits: true),
            //   metrics: new[] { "accuracy" });

        }
        public void Train(string path, int epochs)
        {
            PrepareData(path);
            model.fit(train_ds, validation_data: val_ds, epochs: epochs);
        }
        protected void PrepareData(string pathData)
        {
            // convert to tensor
            train_ds = keras.preprocessing.image_dataset_from_directory(pathData,
                validation_split: 0.2f,
                color_mode: "grayscale",
                subset: "training",
                seed: 123,
                image_size: (img_dim[0], img_dim[1]));

            val_ds = keras.preprocessing.image_dataset_from_directory(pathData,
            validation_split: 0.2f,
            subset: "validation",
                color_mode: "grayscale",
            seed: 123,
                image_size: (img_dim[0], img_dim[1]));

            train_ds = train_ds.shuffle(1000).prefetch(buffer_size: -1);
            val_ds = val_ds.prefetch(buffer_size: -1);
        }
    }
    class WmrFastNNClick : NN
    {
        public WmrFastNNClick(Size imgSize) : base(imgSize)
        {
        }
        public WmrFastNNClick(Size imgSize, string path) : base(imgSize, path)
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
            var inputs = keras.layers.Input(shape: img_dim, name: "img");

            // convolutional layer
            var x = layers.Rescaling(1.0f / 255, input_shape: img_dim).Apply(inputs);
            x = layers.Conv2D(10, 3, activation: "relu").Apply(x);
            x = layers.Conv2D(64, 3, activation: "relu").Apply(x);
            x = layers.Conv2D(64, 3, activation: "relu").Apply(x);
            x = layers.GlobalAveragePooling2D().Apply(x);
            x = layers.Dense(256, activation: "relu").Apply(x);
            x = layers.Dropout(0.5f).Apply(x);

            // output layer
            var outputs = layers.Dense(10).Apply(x);

            // build keras model
            model = keras.Model(inputs, outputs, name: "WmrClick");
            model.summary();

            model.compile(optimizer: keras.optimizers.Adam(),
                loss: keras.losses.SparseCategoricalCrossentropy(from_logits: true),
                metrics: new[] { "accuracy" });
        }
    }
    class WmrFastNNAuth : NN
    {
        public WmrFastNNAuth(Size imgSize) : base(imgSize)
        {
        }
        public WmrFastNNAuth(Size imgSize, string path) : base(imgSize, path)
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
            var inputs = keras.layers.Input(shape: img_dim, name: "img");

            // convolutional layer
            var x = layers.Rescaling(1.0f / 255, input_shape: img_dim).Apply(inputs);
            x = layers.Conv2D(10, 3, activation: "relu").Apply(x);
            x = layers.Conv2D(64, 3, activation: "relu").Apply(x);
            x = layers.Conv2D(64, 3, activation: "relu").Apply(x);
            x = layers.GlobalAveragePooling2D().Apply(x);
            x = layers.Dense(256, activation: "relu").Apply(x);
            x = layers.Dropout(0.5f).Apply(x);

            // output layer
            var outputs = layers.Dense(9).Apply(x);

            // build keras model
            model = keras.Model(inputs, outputs, name: "toy_resnet");
            model.summary();

            // compile keras model in tensorflow static graph
            model.compile(optimizer: keras.optimizers.Adam(),
               loss: keras.losses.SparseCategoricalCrossentropy(from_logits: true),
               metrics: new[] { "accuracy" });

        }
    }
}
