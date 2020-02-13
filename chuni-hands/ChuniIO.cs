using System.Collections.Generic;
using System.Diagnostics;
using System.IO.MemoryMappedFiles;
using System.Security.Principal;

namespace chuni_hands {
    internal static class ChuniIO {
        public static MemoryMappedFile _buffer;
        public static MemoryMappedViewAccessor _accessor;

        private static bool _init = false;

        public static void Send(IList<Sensor> sensors) {
            if (!_init) {
                Initialize();
            }

            var data = new byte[6];
            for (var i = 0; i < 6; ++i) {
                data[i] = (byte) (sensors[i].Active ? 0x80 : 0x00);
            }

            _accessor.WriteArray(0, data, 0, 6);
        }

        private static void Initialize() {
            var security = new MemoryMappedFileSecurity();
            var sid = new SecurityIdentifier(WellKnownSidType.WorldSid, null);
            var account = sid.Translate(typeof(NTAccount)) as NTAccount;

            Debug.Assert(account != null);
            security.AddAccessRule(new System.Security.AccessControl.AccessRule<MemoryMappedFileRights>(account.ToString(), MemoryMappedFileRights.FullControl, System.Security.AccessControl.AccessControlType.Allow));
            _buffer = MemoryMappedFile.CreateOrOpen("Local\\BROKENITHM_SHARED_BUFFER", 1024, MemoryMappedFileAccess.ReadWrite, MemoryMappedFileOptions.None, security, System.IO.HandleInheritability.Inheritable);
            _accessor = _buffer.CreateViewAccessor();

            _init = true;
        }
    }
}
