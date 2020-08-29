using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TensorFlow;
using System.IO;

namespace ObjectMonitor
{
    class ImageUtil
    {
        public static TFTensor CreateTensorFromImageFile(string file, TFDataType destinationDataType = TFDataType.Float)
        {
            var contents = File.ReadAllBytes(file);

            // DecodeJpeg uses a scalar String-valued tensor as input.
            var tensor = TFTensor.CreateString(contents);
            TFOutput input;
            TFOutput output;
            // Construct a graph to normalize the image
            using (var graph = ConstructGraphToNormalizeImage(out input, out output, destinationDataType))
            {
                // Execute that graph to normalize this one image
                using (var session = new TFSession(graph))
                {
                    var normalized = session.Run(
                        inputs: new[] { input },
                        inputValues: new[] { tensor },
                        outputs: new[] { output });

                    return normalized[0];
                }
            }
        }

        public static TFTensor CreateTensorFromMemoryJpg(Byte[] jpeg, TFDataType destinationDataType = TFDataType.Float)
        {
            // DecodeJpeg uses a scalar String-valued tensor as input.

            var tensor = TFTensor.CreateString(jpeg);
            TFOutput input;
            TFOutput output;
            // Construct a graph to normalize the image
            using (var graph = ConstructGraphToNormalizeImage2(out input, out output, destinationDataType))
            {
                // Execute that graph to normalize this one image
                using (var session = new TFSession(graph))
                {
                    var normalized = session.Run(
                        inputs: new[] { input },
                        inputValues: new[] { tensor },
                        outputs: new[] { output });

                    return normalized[0];
                }
            }

        }

        public static TFTensor CreateTensorFromBGR(Byte[] rgb  , TFDataType destinationDataType = TFDataType.Float)
        {
            // DecodeJpeg uses a scalar String-valued tensor as input.
            //var tensor = TFTensor.CreateString(bgr);
  /*
            byte[] floatValues = new byte[640 * 480 * 3];
            int j = 0;
            for (int i = 0; i < rgb.Length/4; i++)
            {
                floatValues[j * 3 + 0] = (rgb[i * 4 + 0]) ;
                floatValues[j * 3 + 1] = (rgb[i * 4 + 1]) ;
                floatValues[j * 3 + 2] = (rgb[i * 4 + 2]) ;
                j++;
            }*/

            TFShape shape = new TFShape(480, 640, 3);

            var tensor = TFTensor.FromBuffer(shape, rgb, 0, rgb.Length);
             TFOutput input;
             TFOutput output;
             // Construct a graph to normalize the image
             using (var graph = ConstructGraphToNormalizeImage(out input, out output, destinationDataType))
             {
                 // Execute that graph to normalize this one image
                 using (var session = new TFSession(graph))
                 {
                     var normalized = session.Run(
                         inputs: new[] { input },
                         inputValues: new[] { tensor },
                         outputs: new[] { output });

                     return normalized[0];
                 }
             }

        }


        private static TFGraph ConstructGraphToNormalizeImage(out TFOutput input, out TFOutput output, TFDataType destinationDataType = TFDataType.Float)
        {
            const int W = 224;
            const int H = 224;
            const float Mean = 117;
            const float Scale = 1;

            var graph = new TFGraph();
            input = graph.Placeholder(TFDataType.UInt8);

            output = graph.Cast(graph.Div(
                x: graph.Sub(
                    x: graph.ResizeBilinear(
                        images: graph.ExpandDims(
                            input: graph.Cast(
                                input, DstT: TFDataType.Float),
                            dim: graph.Const(0, "make_batch")),
                        size: graph.Const(new int[] { W, H }, "size")),
                    y: graph.Const(Mean, "mean")),
                y: graph.Const(Scale, "scale")), destinationDataType);

            
            return graph;
        }

        private static TFGraph ConstructGraphToNormalizeImage2(out TFOutput input, out TFOutput output, TFDataType destinationDataType = TFDataType.Float)
        {
            const int W = 224;
            const int H = 224;
            const float Scale = 1;

            // Depending on your CustomVision.ai Domain - set appropriate Mean Values (RGB)
            // https://github.com/Azure-Samples/cognitive-services-android-customvision-sample for RGB values (in BGR order)
            var bgrValues = new TFTensor(new float[] { 0, 0, 0 }); // General (Compact) & Landmark (Compact)
            //var bgrValues = new TFTensor(0f); // Retail (Compact)

            var graph = new TFGraph();
            input = graph.Placeholder(TFDataType.String);

            var caster = graph.Cast(graph.DecodeJpeg(contents: input, channels: 3), DstT: TFDataType.Float);
            var dims_expander = graph.ExpandDims(caster, graph.Const(0, "batch"));
            var resized = graph.ResizeBilinear(dims_expander, graph.Const(new int[] { H, W }, "size"));
            var resized_mean = graph.Sub(resized, graph.Const(bgrValues, "mean"));
            var normalised = graph.Div(resized_mean, graph.Const(Scale));
            output = graph.Cast(normalised , destinationDataType);
            return graph;
        }
    }
}
