using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Util
{
    public static class ContainInOrder
    {
      public  static bool ContainsInOrder(string main, string search)
        {
            // Split the search string into words
            string[] parts = search.Split(' ');
            int lastIndex = -1;

            foreach (var part in parts)
            {
                // Find the index of the current part in the main string, starting from the last found index + 1
                lastIndex = main.IndexOf(part, lastIndex + 1);
                if (lastIndex == -1)
                {
                    return false;
                }
            }
            return true;
        }
    }
}
