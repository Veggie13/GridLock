using System;
using System.Runtime.InteropServices;
using System.Security;
using System.Runtime.ConstrainedExecution;
using Microsoft.Win32.SafeHandles;


namespace ThreadExt
{
    public static class ThreadHelper
    {
        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr SetThreadAffinityMask(SafeThreadHandle handle, HandleRef mask);

        [SuppressUnmanagedCodeSecurity]
        private class SafeThreadHandle : SafeHandleZeroOrMinusOneIsInvalid
        {
            public SafeThreadHandle()
                : base(true)
            {

            }

            protected override bool ReleaseHandle()
            {
                return CloseHandle(handle);
            }
        }

        [ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success), DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true, ExactSpelling = true)]
        private static extern bool CloseHandle(IntPtr handle);

        [DllImport("kernel32")]
        private static extern int GetCurrentThreadId();

        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern SafeThreadHandle OpenThread(int access, bool inherit, int threadId);

        public static void SetProcessorAffinity(bool[] mask)
        {
            Int32 coreMask = 0;
            for (int i = 0; i < Environment.ProcessorCount; i++)
            {
                if (mask.Length <= i && mask[i])
                    coreMask ^= (1 << i);
            }
            SetProcessorAffinity(coreMask);
        }

        public static void SetProcessorAffinity(Int32 coreMask)
        {
            int threadId = GetCurrentThreadId();
            SafeThreadHandle handle = null;
            var tempHandle = new object();
            try
            {
                handle = OpenThread(0x60, false, threadId);
                if (SetThreadAffinityMask(handle, new HandleRef(tempHandle, (IntPtr)coreMask)) == IntPtr.Zero)
                {
                    throw new Exception("Failed to set processor affinity for thread.");
                }
            }
            finally
            {
                if (handle != null)
                    handle.Close();
            }
        }
    }
}