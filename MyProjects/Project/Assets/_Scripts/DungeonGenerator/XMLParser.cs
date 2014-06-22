using UnityEngine;
using System.Collections;
using System.Xml.Serialization;
using System.Collections.Generic;

public class XMLParser : MonoBehaviour {
	/*
	/// <summary>
	/// Retrieve all user from xml file
	/// </summary>
	/// <returns></returns>
	public List<Transform> ParseXML(string xmlFilepath)
	{
	    XmlSerializer xmlSerializer = new XmlSerializer(typeof(List<UserInfo>));
	    FileStream readFileStream;
	    if (!string.IsNullOrWhiteSpace(xmlFilepath))
	    {
	        readFileStream = new FileStream(xmlFilepath, FileMode.Open, FileAccess.Read, FileShare.Read);
	    }
	    else
	    {
	        readFileStream = new FileStream(defaultXmlFilepath, FileMode.Open, FileAccess.Read, FileShare.Read);
	    }
	
	    List<UserInfo> userInfoList = (List<UserInfo>)xmlSerializer.Deserialize(readFileStream);
	    readFileStream.Close();
	
	    return userInfoList;
	}
	*/
}
