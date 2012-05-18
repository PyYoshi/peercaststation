﻿using System;
using System.Collections.Generic;
using System.IO;

namespace PeerCastStation.Core
{
  /// <summary>
  /// 読み取ったデータをそのままコンテントとして流すクラスです
  /// </summary>
  public class RawContentReader
    : IContentReader
  {
    public RawContentReader(Channel channel)
    {
      this.Channel = channel;
    }

    public ParsedContent Read(Stream stream)
    {
      if (stream.Length-stream.Position<=0) throw new EndOfStreamException();
      var res = new ParsedContent();
      var pos = Channel.ContentPosition;
      if (Channel.ContentHeader==null) {
        res.ContentHeader = new Content(pos, new byte[] { });
        var channel_info = new AtomCollection(Channel.ChannelInfo.Extra);
        channel_info.SetChanInfoType("RAW");
        res.ChannelInfo = new ChannelInfo(channel_info);
      }
      res.Contents = new List<Content>();
      while (stream.Length-stream.Position>0) {
        var bytes = new byte[Math.Min(8192, stream.Length-stream.Position)];
        var sz = stream.Read(bytes, 0, bytes.Length);
        if (sz>0) {
          Array.Resize(ref bytes, sz);
          res.Contents.Add(new Content(pos, bytes));
          pos += sz;
        }
      }
      return res;
    }

    public string  Name    { get { return "RAW"; } }
    public Channel Channel { get; private set; }
  }

  /// <summary>
  /// 読み取ったデータをそのままコンテントとして流すRawContentReaderのファクトリクラスです
  /// </summary>
  [Plugin(PluginPriority.Lower)]
  public class RawContentReaderFactory
    : IContentReaderFactory
  {
    public string Name
    {
      get { return "RAW"; }
    }

    public IContentReader Create(Channel channel)
    {
      return new RawContentReader(channel);
    }
  }
}
