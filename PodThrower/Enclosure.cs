using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using Microsoft.ServiceModel.Samples.Syndication;
using System.Xml.Serialization;

namespace PodThrower
{
	[DataContract(Name = FeedConstants.Category, Namespace = FeedConstants.FeedNamespace)]
	[XmlType(TypeName = FeedConstants.Category, Namespace = FeedConstants.FeedNamespace)]
	public class Enclosure
	{
		public Enclosure(string domain, string name)
		{
			CategoryDomain = domain;
			CategoryValue = name;
		}

		[DataMember(Name = FeedConstants.CategoryDomain)]
		[XmlElement(ElementName = FeedConstants.CategoryDomain)]
		public string CategoryDomain
		{
			get;
			set;
		}

		[DataMember(Name = FeedConstants.CategoryValue)]
		[XmlElement(ElementName = FeedConstants.CategoryValue)]
		public string CategoryValue
		{
			get;
			set;
		}
	}
}
