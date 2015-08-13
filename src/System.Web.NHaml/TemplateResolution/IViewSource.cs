using System.IO;
using System.Text;

namespace System.Web.NHaml.TemplateResolution
{
    /// <summary>
    /// Represents a view template source
    /// </summary>
    public abstract class ViewSource
    {
        public abstract TextReader GetTextReader();
        public abstract string FilePath { get; }
        public abstract string FileName { get; }
        public abstract DateTime TimeStamp { get; }

        string _className = null;

        public string ClassName{
            get{
                return GetClassName();
            }
            set{
                _className = value;
            }
        }
        
        public string GetClassName()
        {
            if (_className != null) return _className;

            string templatePath = FilePath;
            var stringBuilder = new StringBuilder();
            foreach (char ch in templatePath)
            {
                stringBuilder.Append(Char.IsLetter(ch) ? ch : '_');
            }

            return stringBuilder.ToString();
        }
    }
}