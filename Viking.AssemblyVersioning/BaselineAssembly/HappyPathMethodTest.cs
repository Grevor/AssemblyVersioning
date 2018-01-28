namespace BaselineAssembly
{
    public class HappyPathMethodTest
    {
        public void EmptyMethod() { }
        public void SingleParamMethod(int i) { }
        public void SingleParamMethod(ref int i) { }


        public void EmptyMethod(params int[] i) { }
        public void EmptyMethod(int ii, params int[] i) { }
    }
}
