﻿using System;
using System.Collections.Generic;
using Foundation;
using Xamarin.Forms;

using StreetHawkCrossplatform;
using StreethawkIOS.Feed;

[assembly: Dependency(typeof(StreetHawkFeeds))]

namespace StreetHawkCrossplatform
{
	public class StreetHawkFeeds : IStreetHawkFeeds
	{
		public void SendFeedAck(string feedid)
		{
			SHFeed.instance().sendFeedAck(feedid);
		}

		public void NotifyFeedResult(string feedid, int result)
		{
			SHFeed.instance().sendLogForFeed(feedid, (StreethawkIOS.Feed.SHFeedResult)result);
		}

		public void NotifyFeedResult(string feedid, string stepid, string feedresult, bool feedDelete, bool completed)
		{
			StreethawkIOS.Feed.SHFeedResult result = SHFeedResult.SHResult_Accept;
			if (feedresult.Equals("postponed"))
			{
				result = SHFeedResult.SHResult_Postpone;
			}
			else if (feedresult.Equals("rejected"))
			{
				result = SHFeedResult.SHResult_Decline;
			}
			SHFeed.instance().notifyFeedResult(feedid, result, stepid, feedDelete, completed);
		}

		public void ReadFeedData(int offset, RegisterForFeedCallback cb)
		{
			SHFeed.instance().feed(offset, delegate (NSArray arrayDicts, NSError error)
			  {
				  if (cb != null)
				  {
					  if (error == null)
					  {
						  List<SHFeedObject> ret = new List<SHFeedObject>();
						  for (nuint i = 0; i < arrayDicts.Count; i++)
						  {
							  NSDictionary feedDict = arrayDicts.GetItem<NSDictionary>(i);
							  SHFeedObject feed = new SHFeedObject();
							  if (feedDict["feed_id"] != null)
							  {
								  feed.feed_id = feedDict["feed_id"].ToString();
							  }
							  if (feedDict["title"] != null)
							  {
								  feed.title = feedDict["title"].ToString();
							  }
							  if (feedDict["message"] != null)
							  {
								  feed.message = feedDict["message"].ToString();
							  }
							  if (feedDict["campaign"] != null)
							  {
								  feed.campaign = feedDict["campaign"].ToString();
							  }
							  if (feedDict["content"] != null)
							  {
								  NSError contentError;
								  NSData contentData = NSJsonSerialization.Serialize(feedDict["content"]/*NSDictionary type*/, 0, out contentError);
								  NSString contentStr = new NSString(contentData, NSStringEncoding.UTF8);
								  feed.content = contentStr.ToString();
							  }
							  if (feedDict["activates"] != null)
							  {
								  feed.activates = feedDict["activates"].ToString();
							  }
							  if (feedDict["expires"] != null)
							  {
								  feed.expires = feedDict["expires"].ToString();
							  }
							  if (feedDict["created"] != null)
							  {
								  feed.created = feedDict["created"].ToString();
							  }
							  if (feedDict["modified"] != null)
							  {
								  feed.modified = feedDict["modified"].ToString();
							  }
							  if (feedDict["deleted"] != null)
							  {
								  feed.deleted = feedDict["deleted"].ToString();
							  }
							  ret.Add(feed);
						  }
						  cb(ret, null);
					  }
					  else
					  {
						  cb(null, error.LocalizedDescription);
					  }
				  }
			  });
		}

		public void OnNewFeedAvailableCallback(RegisterForNewFeedCallback cb)
		{
			SHFeed.instance().newFeedHandler = new StreethawkIOS.Feed.SHNewFeedsHandler(cb);
		}
	}
}

