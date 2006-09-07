using System;
using System.Collections.Generic;
using System.Text;
using ID3COM;
using System.Security;

namespace Sycorax {
    /// <summary>
    /// Lecture et manipulation de tags id3
    /// </summary>
    static public class id3lib {
        /// <summary>
        /// Gets the id3 tag.
        /// </summary>
        /// <param name="file">The file to parse.</param>
        /// <returns>id3 tag</returns>
        static public ID3ComTagClass GetTag (string file) {
            ID3ComTagClass Tag = new ID3ComTagClass();
            try {
                //Tentons de relier notre classer tag au fichier
                Tag.Link(ref file);
            } catch {
                //en cas d'erreur renvoyons une valeur nulle
                return null;
            }
            //Tout se passe bien, nous pouvons le renvoyer
            return Tag;
        }
    }
}
