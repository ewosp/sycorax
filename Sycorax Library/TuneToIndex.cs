using System;
using System.Text;

namespace Sycorax {
    /// <summary>
    /// Tune to index in our database
    /// </summary>
    public class TuneToIndex {
        /// <summary>
        /// Title
        /// </summary>
        public string Title = "";

        /// <summary>
        /// Singer, group, ...
        /// </summary>
        public string By = "";

        /// <summary>
        /// Comment
        /// </summary>
        public string Comment = "";

        /// <summary>
        /// File path
        /// </summary>
        public string Path = "";

        /// <summary>
        /// Returns a 'By - Title (Comment)' <see cref="T:System.String"></see> that represents the current <see cref="T:System.Object"></see>.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.String"></see> that represents the current <see cref="T:System.Object"></see>.
        /// By - Title (Comment)
        /// </returns>
        public override string ToString () {
            StringBuilder sb = new StringBuilder();
            if (By.Length > 0) {
                sb.Append(By);
                sb.Append(" - ");
            }
            sb.Append(Title);
            if (Comment.Length > 0) {
                sb.Append("(");
                sb.Append(Comment);
                sb.Append(")");
            }
            return sb.ToString();
        }
    }
}
