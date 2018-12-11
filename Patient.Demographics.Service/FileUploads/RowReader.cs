using System.IO;

namespace Patient.Demographics.Service.FileUploads
{
    public class RowReader : ReadLineTextReader
    {

        private int currentRow = -1;

        private readonly string[] _rows;

        public RowReader(string[] rows)
        {
            _rows = rows;
        }



        public override string ReadLine()
        {

            currentRow ++;

            if (currentRow < _rows.Length)
            {
                return _rows[currentRow];
            }
            else
            {
                return null;
            }

        }
    }

    // pinched from  https://blogs.msdn.microsoft.com/jmstall/2005/08/06/deriving-from-textreader/
    public abstract class ReadLineTextReader : TextReader
    {

        int _index = int.MaxValue;
        string _line;


        public override int Peek()
        {
            FillCharCache();
            return _charCache;
        }

        public override int Read()
        {
            FillCharCache();
            int ch = _charCache;
            ClearCharCache();
            return ch;
        }

        int _charCache = -2; // -2 means the cache is empty. -1 means eof.
        void ClearCharCache()
        {
            _charCache = -2;
        }
        void FillCharCache()
        {
            if (_charCache != -2) return; // cache is already full
            _charCache = GetNextCharWorker();
        }

        // The whole point of this helper class is that the derived class is going to 
        // implement ReadLine() instead of Read(). So mark that we don’t want to use TextReader’s 
        // default implementation of ReadLine(). Null return means eof.
        public abstract override string ReadLine();

        int GetNextCharWorker()
        {

            if (_line == null)
            {
                _line = ReadLine();
                _index = 0;
                if (_line == null)
                {
                    return -1;
                }
                _line += "\r\n";
            }
            char c = _line[_index];
            _index++;
            if (_index >= _line.Length)
            {
                _line = null;
            }
            return c;
        }
    }
}