using System;

namespace OpusNet
{
    public enum Ctl : int
    {
        SetBitrateRequest = 4002,
        GetBitrateRequest = 4003
    }

    /// <summary>
    /// Supported coding modes.
    /// </summary>
    public enum Application
    {
        /// <summary>
        /// Best for most VoIP/videoconference applications where listening quality and intelligibility matter most.
        /// </summary>
        Voip = 2048,
        /// <summary>
        /// Best for broadcast/high-fidelity application where the decoded audio should be as close as possible to input.
        /// </summary>
        Audio = 2049,
        /// <summary>
        /// Only use when lowest-achievable latency is what matters most. Voice-optimized modes cannot be used.
        /// </summary>
        Restricted_LowLatency = 2051
    }

    public enum Errors
    {
        /// <summary>
        /// No error.
        /// </summary>
        OK              = 0,
        /// <summary>
        /// One or more invalid/out of range arguments.
        /// </summary>
        BadArg          = -1,
        /// <summary>
        /// The mode struct passed is invalid.
        /// </summary>
        BufferToSmall   = -2,
        /// <summary>
        /// An internal error was detected.
        /// </summary>
        InternalError   = -3,
        /// <summary>
        /// The compressed data passed is corrupted.
        /// </summary>
        InvalidPacket   = -4,
        /// <summary>
        /// Invalid/unsupported request number.
        /// </summary>
        Unimplemented   = -5,
        /// <summary>
        /// An encoder or decoder structure is invalid or already freed.
        /// </summary>
        InvalidState    = -6,
        /// <summary>
        /// Memory allocation has failed.
        /// </summary>
        AllocFail       = -7
    }
}
