﻿using System;
using System.Data;
using System.IO;
using System.Xml.Serialization;
using System.Xml;
using System.Collections.Generic;

namespace HY.SpliderService
{
    public class XmlHelper
    {

        #region 反序列化
        /// <summary>
        /// 反序列化
        /// </summary>
        /// <param name="type">类型</param>
        /// <param name="xml">XML字符串</param>
        /// <returns></returns>
        public static object Deserialize(Type type, string xml)
        {
            try
            {
                using (StringReader sr = new StringReader(xml))
                {
                    XmlSerializer xmldes = new XmlSerializer(type);
                    return xmldes.Deserialize(sr);
                }
            }
            catch
            {
                throw new Exception("反序列化失败");
            }
        }
        /// <summary>
        /// 反序列化
        /// </summary>
        /// <param name="type"></param>
        /// <param name="xml"></param>
        /// <returns></returns>
        public static object Deserialize(Type type, Stream stream)
        {
            XmlSerializer xmldes = new XmlSerializer(type);
            return xmldes.Deserialize(stream);
        }
        #endregion

        #region 序列化XML文件
        /// <summary>
        /// 序列化XML文件
        /// </summary>
        /// <param name="type">类型</param>
        /// <param name="obj">对象</param>
        /// <returns></returns>
        public static string Serializer(Type type, object obj)
        {
            MemoryStream Stream = new MemoryStream();
            //创建序列化对象
            XmlSerializer xml = new XmlSerializer(type);
            try
            {
                //序列化对象
                xml.Serialize(Stream, obj);
            }
            catch (InvalidOperationException)
            {
                throw;
            }
            Stream.Position = 0;
            StreamReader sr = new StreamReader(Stream);
            string str = sr.ReadToEnd();
            return str;
        }
        #endregion

        #region 将XML转换为DATATABLE
        /// <summary>
        /// 将XML转换为DATATABLE
        /// </summary>
        /// <param name="FileURL"></param>
        /// <returns></returns>
        public static DataTable XmlAnalysisArray()
        {
            try
            {
                string FileURL = System.Configuration.ConfigurationManager.AppSettings["Client"].ToString();
                DataSet ds = new DataSet();
                ds.ReadXml(FileURL);
                return ds.Tables[0];
            }
            catch (Exception ex)
            {
                //System.Web.HttpContext.Current.Response.Write(ex.Message.ToString());
                return null;
            }
        }
        /// <summary>
        /// 将XML转换为DATATABLE
        /// </summary>
        /// <param name="FileURL"></param>
        /// <returns></returns>
        public static DataTable XmlAnalysisArray(string FileURL)
        {
            try
            {
                DataSet ds = new DataSet();
                ds.ReadXml(FileURL);
                return ds.Tables[0];
            }
            catch (Exception ex)
            {
                //System.Web.HttpContext.Current.Response.Write(ex.Message.ToString());
                return null;
            }
        }
        #endregion

        #region 获取对应XML节点的值
        /// <summary>
        /// 摘要:获取对应XML节点的值
        /// </summary>
        /// <param name="stringRoot">XML节点的标记</param>
        /// <returns>返回获取对应XML节点的值</returns>
        public static string XmlAnalysis(string stringRoot, string xml)
        {
            if (stringRoot.Equals("") == false)
            {
                try
                {
                    XmlDocument XmlLoad = new XmlDocument();
                    XmlLoad.LoadXml(xml);
                    return XmlLoad.DocumentElement.SelectSingleNode(stringRoot).InnerXml.Trim();
                }
                catch
                {
                    throw new Exception("解析XML失败");
                }
            }
            return "";
        }
        #endregion

        /// <summary>
        /// 判断文章是否重复抓取
        /// </summary>
        /// <param name="articleID"></param>
        /// <returns></returns>
        public bool IsCatch(string articleID)
        {
            var list = new List<XmlLog>();
            XmlDocument doc = new XmlDocument();
            doc.Load(@"SpliderLog\splider.xml");
            XmlNode xn = doc.SelectSingleNode("log");
            //得到根节点的所有子节点
            XmlNodeList xnl = xn.ChildNodes;
            foreach (XmlNode item in xnl)
            {
                XmlLog log = new XmlLog();
                // 得到节点的所有子节点
                XmlNodeList XmlLog = item.ChildNodes;
                var ArticleID = XmlLog.Item(0).InnerText;
                if (ArticleID == articleID)
                {
                    return true;
                }
            }
            return false;
        }
        /// <summary>
        /// 抓取完成，写XML日志
        /// </summary>
        /// <param name="ArticleID"></param>
        /// <param name="createDate"></param>
        public void InsertLog(string ArticleID, DateTime createDate)
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(@"SpliderLog\splider.xml");
            XmlNode root = doc.SelectSingleNode("log");
            XmlNode log = doc.CreateElement("XmlLog");
            XmlNode xmlArticleID = doc.CreateElement("ArticleID");
            xmlArticleID.InnerText = ArticleID;
            log.AppendChild(xmlArticleID);
            XmlNode xmlCreateDate = doc.CreateElement("CreateDate");
            xmlCreateDate.InnerText = createDate.ToString("yyyy-MM-dd HH:mm:ss");
            log.AppendChild(xmlCreateDate);
            root.AppendChild(log);
            if (root.ChildNodes.Count > 2)
            {
                XmlNode DelNode = root.ChildNodes[0];
                root.RemoveChild(DelNode);
            }
            doc.Save(@"SpliderLog\splider.xml");
        }
    }
}
