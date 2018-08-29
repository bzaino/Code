///////////////////////////////////////////////
//  WorkFile Name: TranslationHelper.cs in ASA.WCFExtensions
//  Description:   
//          Helper class containing functions for helping translate from Business Objects to Data Contracts
//     
//            ASA Proprietary Information
///////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.Text;

namespace ASA.WCFExtensions
{
    public class TranslationHelper
    {
        public delegate T1 TranslateListDelegate<T1, U1>(U1 objectToTranslate);

        /// <summary>
        /// Helps with the translation of list of business objects to a list of data contracts or vice versa
        /// </summary>
        /// <param name="listToTranslate">Generic list of objects needing translation</param>
        /// <param name="translatedList">Generic list of translated objects</param>
        /// <param name="translateFunction">Generic delegate for translation function</param>
        /// <typeparam name="T">Type of objects in list to be translated</typeparam>
        /// <typeparam name="U">Type of objects to be translated to</typeparam>
        /// <returns>Boolean indicating success or failure</returns>
        public static List<U> TranslateList<T, U>(IList<T> listToTranslate, TranslateListDelegate<U, T> translateFunction)
        {

            List<U> translatedList = new List<U>();
            
            if (listToTranslate == null)
                return translatedList;

            foreach (T objectToTranslate in listToTranslate)
            {
                translatedList.Add(translateFunction(objectToTranslate));
            }

            return translatedList;
        }
    }
}
