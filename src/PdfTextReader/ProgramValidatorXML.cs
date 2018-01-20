﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml.Linq;

namespace PdfTextReader
{
    class ProgramValidatorXML
    {
        public void ValidateArticle(string inputfolder)
        {
            var directory = new DirectoryInfo(inputfolder);
            foreach (var file in directory.EnumerateFiles("*.xml"))
            {
                Validate(file);
            }
        }

        void Validate(FileInfo file)
        {
            if (!CheckBody(file) || !CheckTitle(file) || CheckHierarchy(file))
                file.CopyTo($"bin/{file.Name.Replace(".xml", "")}-ISSUE.xml");
        }

        bool CheckHierarchy(FileInfo file)
        {
            string text = null;
            XDocument doc = XDocument.Load(file.FullName);
            foreach (XElement el in doc.Root.Elements())
            {
                foreach (XAttribute item in el.Attributes())
                {
                    if (item.Name == "Hierarquia")
                        text = item.Value;
                }
            }

            if (text == null)
            {
                //Não tem hierarquia
                return true;
            }
            else
            {
                if (text.Replace("o", "O").ToUpper() == text.Replace("o", "O"))
                    return true;
                return false;
            }
        }

        bool CheckTitle(FileInfo file)
        {
            string text = null;
            XDocument doc = XDocument.Load(file.FullName);
            foreach (XElement el in doc.Root.Elements())
            {
                foreach (XElement item in el.Elements())
                {
                    if (item.Name == "Titulo")
                        text = item.Value;
                }
            }


            if (text != null)
            {
                if (text.Replace("o", "O").ToUpper() == text.Replace("o", "O"))
                    return true;
            }

            return false;
        }

        bool CheckBody(FileInfo file)
        {
            string text = null;
            XDocument doc = XDocument.Load(file.FullName);
            foreach (XElement el in doc.Root.Elements())
            {
                foreach (XElement item in el.Elements())
                {
                    if (item.Name == "Corpo")
                        text = item.Value;

                }
            }
            if (text != null)
            {
                return true;
            }
            return false;
        }
    }
}
