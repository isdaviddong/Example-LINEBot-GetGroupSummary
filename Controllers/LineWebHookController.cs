using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace isRock.Template
{
    public class LineWebHookController : isRock.LineBot.LineWebHookControllerBase
    {
        [Route("api/LineBotWebHook")]
        [HttpPost]
        public IActionResult POST()
        {
            var AdminUserId = "______________AdminUserId_________";

            try
            {
                //設定ChannelAccessToken
                this.ChannelAccessToken = "_____________ChannelAccessToken____________";
                //取得Line Event
                var LineEvent = this.ReceivedMessage.events.FirstOrDefault();
                //配合Line verify 
                if (LineEvent.replyToken == "00000000000000000000000000000000") return Ok();
                var responseMsg = "";
                //準備回覆訊息
                if (LineEvent.type.ToLower() == "message" && LineEvent.message.type == "text")
                {
                    //在群組裡
                    if (!string.IsNullOrEmpty(LineEvent.source.groupId))
                    {
                        //when join to group
                        var info = this.GetGroupSummary(LineEvent.source.groupId);
                        responseMsg = "GroupSummary : " + Newtonsoft.Json.JsonConvert.SerializeObject(info);
                        var Count = this.GetMembersInGroupCount(LineEvent.source.groupId);
                        responseMsg += "\nGetMembersInRoomCount : " + Count;
                    }
                    //在聊天室裡
                    if (!string.IsNullOrEmpty(LineEvent.source.roomId))
                    {
                        //when join to room
                        var Count = this.GetMembersInRoomCount(LineEvent.source.roomId);
                        responseMsg = "GetMembersInRoomCount : " + Count;
                    }
                    responseMsg += $"\n你說了: {LineEvent.message.text}";
                }
                else if (LineEvent.type.ToLower() == "message")
                    responseMsg = $"收到 event : {LineEvent.type} type: {LineEvent.message.type} ";
                else if (LineEvent.type.ToLower() == "join" && !string.IsNullOrEmpty(LineEvent.source.groupId))
                {
                    //被加入群組
                    var info = this.GetGroupSummary(LineEvent.source.groupId);
                    responseMsg = "GroupSummary : " + Newtonsoft.Json.JsonConvert.SerializeObject(info);
                    var Count = this.GetMembersInGroupCount(LineEvent.source.groupId);
                    responseMsg += "\nGetMembersInRoomCount : " + Count;
                }
                else if (LineEvent.type.ToLower() == "join" && !string.IsNullOrEmpty(LineEvent.source.roomId))
                {
                    //被加入聊天室
                    var Count = this.GetMembersInRoomCount(LineEvent.source.roomId);
                    responseMsg = "GetMembersInRoomCount : " + Count;
                }
                else
                    responseMsg = $"收到 event : {LineEvent.type} ";
                //回覆訊息
                this.ReplyMessage(LineEvent.replyToken, responseMsg);
                //response OK
                return Ok();
            }
            catch (Exception ex)
            {
                //回覆訊息
                this.PushMessage(AdminUserId, "發生錯誤:\n" + ex.Message);
                //response OK
                return Ok();
            }
        }
    }
}
