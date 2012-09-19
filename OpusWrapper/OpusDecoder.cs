using System;
using FragLabs.Audio.Codecs.Opus;

namespace FragLabs.Audio.Codecs
{
    /// <summary>
    /// Opus audio decoder.
    /// </summary>
    public class OpusDecoder : IDisposable
    {
        /// <summary>
        /// Creates a new Opus decoder.
        /// </summary>
        /// <param name="outputSampleRate">Sample rate to decode at (Hz). This must be one of 8000, 12000, 16000, 24000, or 48000.</param>
        /// <param name="outputChannels">Number of channels to decode.</param>
        /// <returns>A new <c>OpusDecoder</c>.</returns>
        public static OpusDecoder Create(int outputSampleRate, int outputChannels)
        {
            if (outputSampleRate != 8000 &&
                outputSampleRate != 12000 &&
                outputSampleRate != 16000 &&
                outputSampleRate != 24000 &&
                outputSampleRate != 48000)
                throw new ArgumentOutOfRangeException("inputSamplingRate");
            if (outputChannels != 1 && outputChannels != 2)
                throw new ArgumentOutOfRangeException("inputChannels");

            IntPtr error;
            IntPtr decoder = API.opus_decoder_create(outputSampleRate, outputChannels, out error);
            if ((Errors)error != Errors.OK)
            {
                throw new Exception("Exception occured while creating decoder");
            }
            return new OpusDecoder(decoder, outputSampleRate, outputChannels);
        }

        private IntPtr _decoder;

        private OpusDecoder(IntPtr decoder, int outputSamplingRate, int outputChannels)
        {
            _decoder = decoder;
            OutputSamplingRate = outputSamplingRate;
            OutputChannels = outputChannels;
            MaxDataBytes = 4000;
        }

        /// <summary>
        /// Produces PCM samples from Opus encoded data.
        /// </summary>
        /// <param name="inputOpusData">Opus encoded data to decode.</param>
        /// <returns>PCM audio samples.</returns>
        public unsafe byte[] Decode(byte[] inputOpusData)
        {
            if (disposed)
                throw new ObjectDisposedException("OpusDecoder");

            IntPtr decodedPtr;
            var rand = new Random();
            byte[] decoded = new byte[MaxDataBytes];
            rand.NextBytes(decoded);
            int frameCount = FrameCount(MaxDataBytes);
            fixed (byte* bdec = decoded)
                decodedPtr = new IntPtr((void*)bdec);

            int length = API.opus_decode(_decoder, inputOpusData, inputOpusData.Length, decodedPtr, frameCount, 0);
            if (length < 0)
                throw new Exception("Decoding failed - " + ((Errors)length).ToString());

            byte[] fixedDecoded = new byte[length * 2];
            Array.Copy(decoded, fixedDecoded, length * 2);
            return fixedDecoded;
        }

        /// <summary>
        /// Determines the number of frames that can fit into a buffer of the given size.
        /// </summary>
        /// <param name="bufferSize"></param>
        /// <returns></returns>
        public int FrameCount(int bufferSize)
        {
            //  seems like bitrate should be required
            int bitrate = 16;
            int bytesPerSample = (bitrate / 8) * OutputChannels;
            return bufferSize / bytesPerSample;
        }

        /// <summary>
        /// Gets the output sampling rate of the decoder.
        /// </summary>
        public int OutputSamplingRate { get; private set; }

        /// <summary>
        /// Gets the number of channels of the decoder.
        /// </summary>
        public int OutputChannels { get; private set; }

        /// <summary>
        /// Gets or sets the size of memory allocated for decoding data.
        /// </summary>
        public int MaxDataBytes { get; set; }

        ~OpusDecoder()
        {
            Dispose();
        }

        private bool disposed;
        public void Dispose()
        {
            if (disposed)
                return;

            GC.SuppressFinalize(this);

            if (_decoder != IntPtr.Zero)
            {
                API.opus_decoder_destroy(_decoder);
                _decoder = IntPtr.Zero;
            }

            disposed = true;
        }
    }
}
