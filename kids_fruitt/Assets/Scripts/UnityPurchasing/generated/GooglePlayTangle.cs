// WARNING: Do not modify! Generated file.

namespace UnityEngine.Purchasing.Security {
    public class GooglePlayTangle
    {
        private static byte[] data = System.Convert.FromBase64String("U9De0eFT0NvTU9DQ0RRz/mO7A0suGC5xeZ8k5XxZ/MKQpace3qQ9U4gje4ZAG0mr5BlAP5thb8B8+gyuh2yJjcp8nku9+p4GoydWbc173SDhU9Dz4dzX2PtXmVcm3NDQ0NTR0m1KZI8/LpdUMhiQdArGECvUT/z4/Ch0MScrJU5bhZHO9YSPk6PQLw49/svln2J2m2hvdpA1Y2JJ+qqT78ViDM6Ud77SbGF4nUG0qYBNxVSw4tnS2pfJaCFPT9QOp1anaWcJpf01aTrbVki9OX9D7/jna2dyVAJRtV9GyXj4gz4piouTz3CCXBqKhvosh+JywgerVYNZxz5+I+FYqByNRkNfem/jhf8/uepq3RgX5MviLoJXGZ29aJOrBVmAQtPS0NHQ");
        private static int[] order = new int[] { 1,10,7,11,10,6,8,13,12,12,12,12,13,13,14 };
        private static int key = 209;

        public static readonly bool IsPopulated = true;

        public static byte[] Data() {
        	if (IsPopulated == false)
        		return null;
            return Obfuscator.DeObfuscate(data, order, key);
        }
    }
}
