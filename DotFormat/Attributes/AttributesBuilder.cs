using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotFormat
{
    public class AttributesBuilder<T>
        where T : class
    {
        protected Dictionary<string, string> Attributes = new();

        public T Color(string color)
        {
            Attributes.Add("color", color);
            return this as T;
        }

        public T FontSize(int fontSize)
        {
            Attributes.Add("fontsize", fontSize.ToString());
            return this as T;
        }

        public T FontColor(string fontSize)
        {
            Attributes.Add("fontcolor", fontSize);
            return this as T;
        }


        public T Label(string text)
        {
            Attributes.Add("label", $" {text}  ");
            return this as T;
        }
    }
}
