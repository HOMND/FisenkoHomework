using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Xml;
using HtmlAgilityPack;


namespace FisenkoHomework
{
    class Program
    {
        public static void Main(string[] args)
        {
            Graph GR = new Graph();
            SiteLoad CODE = new SiteLoad();
            CODE.GetRefs("", GR, CODE);
            RankCounter(GR);
            Console.WriteLine("DONE");

        }
        public static void RankCounter(Graph GR)
        {
            foreach (Vertex tmpage in GR.allpages)
            {
                foreach (Vertex tmpage2 in tmpage.outrefs)
                {
                    tmpage2.rank++;
                }
            }

            foreach (Vertex tmpage in GR.allpages)
            {
                Console.WriteLine(tmpage.thisref);
                Console.WriteLine(tmpage.rank.ToString());

            }

        }

    }

    public class SiteLoad
    {
        static int count = 0;
        //главная страница
        static string htmlstart = "http://www.lebedev.ru";
        public Vertex GetRefs(string html, Graph GR, SiteLoad CODE)
        {
            Vertex alrd = null;
            int cutstr;
            string realhtml;
            bool newpage;

            //начинаю с главной
            if (html == "") html = htmlstart;
            newpage = true;
            //проверяю, не добавлял ли еще
            foreach (Vertex tmpage in GR.allpages) if (html == tmpage.thisref)
                {
                    newpage = false;
                    alrd = tmpage;
                }
            //чтоб не качать всестраницы с сайта, их там больше тысячи
            if (count > 500)
            {
                newpage = false;
                alrd = GR.allpages[0];
            }
            //если добавлял то
            if (newpage)
            {
                //содаю вершину
                Vertex PAGE = new Vertex(html);
                //закидываю ее в граф
                GR.allpages.Add(PAGE);
                //счетчик
                count++;
                Console.WriteLine(count.ToString());

                //вытаскиваю ссылки с сайта
                HtmlWeb web = new HtmlWeb();
                var htmlDoc = web.Load(html);
                foreach (HtmlNode link in htmlDoc.DocumentNode.SelectNodes("//a[@href]"))
                {
                    //обработка строк с сылками
                    realhtml = link.OuterHtml;
                    cutstr = realhtml.IndexOf("\"") + 1;
                    realhtml = realhtml.Substring(cutstr);
                    cutstr = realhtml.IndexOf("\"");
                    realhtml = realhtml.Substring(0, cutstr);
                    realhtml = realhtml.Replace(htmlstart, "");
                    //если ссылка внутренняя - иду по графу вглубину
                    if (realhtml.Length > 4)
                    {
                        if (realhtml.Substring(0, 4) == "/ru/")
                        {
                            realhtml = htmlstart + realhtml;
                            PAGE.AddEdge(GetRefs(realhtml, GR, CODE));
                        }
                    }
                }
                return PAGE;
            }
            return alrd;
        }
    }
    public class Vertex
    {
        //ссылка данной страницы
        public string thisref;
        //ссылки с этой страницы
        public List<Vertex> outrefs;
        //ранг
        public int rank;

        public Vertex(string thrf)
        {
            thisref = thrf;
            outrefs = new List<Vertex>();
            rank = 0;
        }

        public void AddEdge(Vertex oref)
        {
            bool newlink = true;
            //проверка на то, не была ли эта ссылка уже добавлена
            foreach (Vertex tmlink in outrefs) if (oref.thisref == tmlink.thisref) newlink = false;
            if (newlink) outrefs.Add(oref);
        }
    }

    public class Graph
    {
        //список всех страниц (вершин)
        public List<Vertex> allpages;
        public Graph()
        {
            allpages = new List<Vertex>();
            allpages.Add(new Vertex("NULLPAGE"));
        }
    }
}
