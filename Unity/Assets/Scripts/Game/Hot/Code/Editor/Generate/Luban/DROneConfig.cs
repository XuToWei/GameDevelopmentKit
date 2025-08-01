
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using System.Collections.Generic;
using SimpleJSON;
using Luban;

namespace Game.Hot.Editor
{

public sealed class DROneConfig :  Luban.EditorBeanBase 
{
    public DROneConfig()
    {
            GameId = "";
    }

    public override void LoadJson(SimpleJSON.JSONObject _json)
    {
        { 
            var _fieldJson = _json["GameId"];
            if (_fieldJson != null)
            {
                if(!_fieldJson.IsString) { throw new SerializationException(); }  GameId = _fieldJson;
            }
        }
        
        { 
            var _fieldJson = _json["SceneMenu"];
            if (_fieldJson != null)
            {
                if(!_fieldJson.IsNumber) { throw new SerializationException(); }  SceneMenu = _fieldJson;
            }
        }
        
        { 
            var _fieldJson = _json["SceneMain"];
            if (_fieldJson != null)
            {
                if(!_fieldJson.IsNumber) { throw new SerializationException(); }  SceneMain = _fieldJson;
            }
        }
        
    }

    public override void SaveJson(SimpleJSON.JSONObject _json)
    {
        {

            if (GameId == null) { throw new System.ArgumentNullException(); }
            _json["GameId"] = new JSONString(GameId);
        }
        {
            _json["SceneMenu"] = new JSONNumber(SceneMenu);
        }
        {
            _json["SceneMain"] = new JSONNumber(SceneMain);
        }
    }

    public static DROneConfig LoadJsonDROneConfig(SimpleJSON.JSONNode _json)
    {
        DROneConfig obj = new DROneConfig();
        obj.LoadJson((SimpleJSON.JSONObject)_json);
        return obj;
    }
        
    public static void SaveJsonDROneConfig(DROneConfig _obj, SimpleJSON.JSONNode _json)
    {
        _obj.SaveJson((SimpleJSON.JSONObject)_json);
    }

    public string GameId;

    public int SceneMenu;

    public int SceneMain;

}
}

