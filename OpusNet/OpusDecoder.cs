using System;

namespace OpusNet
{
    /// <summary>
    /// Opus audio decoder.
    /// </summary>
    public class OpusDecoder : IDisposable
    {
        private IntPtr _decoder;
        
        static OpusDecoder()
        {
            Wrapper.Initialize();
        }

        /// <summary>
        /// Creates a new Opus decoder.
        /// </summary>
        /// <param name="outputSampleRate">Sample rate to decode at (Hz). This must be one of 8000, 12000, 16000, 24000, or 48000.</param>
        /// <param name="outputChannels">Number of channels to decode.</param>
        /// <returns>A new <c>OpusDecoder</c>.</returns>
        public OpusDecoder(int outputSampleRate, int outputChannels)
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
            this._decoder = Wrapper.opus_decoder_create(outputSampleRate, outputChannels, out error);
            if ((Errors)error != Errors.OK)
                throw new Exception("Exception occured while creating decoder");

            this.OutputSamplingRate = outputSampleRate;
            this.OutputChannels = outputChannels;
            this.MaxDataBytes = 4000;
        }

        /// <summary>
        /// Produces PCM samples from Opus encoded data.
        /// </summary>
        /// <param name="inputOpusData">Opus encoded data to decode.</param>
        /// <param name="dataLength">Length of data to decode.</param>
        /// <param name="decodedLength">Set to the length of the decoded sample data.</param>
        /// <returns>PCM audio samples.</returns>
        public unsafe byte[] Decode(byte[] inputOpusData, int dataLength, out int decodedLength)
        {
            if (disposed)
                throw new ObjectDisposedException("OpusDecoder");

            IntPtr decodedPtr;
            byte[] decoded = new byte[MaxDataBytes];
            int frameCount = FrameCount(MaxDataBytes);
            int length = 0;
            fixed (byte* bdec = decoded)
            {
                decodedPtr = new IntPtr((void*)bdec);
                length = Wrapper.opus_decode(_decoder, inputOpusData, dataLength, decodedPtr, frameCount, 0);
            }
            decodedLength = length * 2;
            if (length < 0)
                throw new Exception("Decoding failed - " + ((Errors)length).ToString());

            return decoded;
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
                Wrapper.opus_decoder_destroy(_decoder);
                _decoder = IntPtr.Zero;
            }

            disposed = true;
        }
    }
}
