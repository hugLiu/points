/// <summary>
/// 删除文件夹
/// </summary>
private void DeleteFolder()
{ 
    try
    {
        var physicalPath = Request.PhysicalApplicationPath.TrimEnd('\\');
        DirectoryInfo dir = new DirectoryInfo(physicalPath + "\\UploadGDBX\\");
        var fileInfo = dir.GetFileSystemInfos();
        foreach (var i in fileInfo)
        {
            if (i is DirectoryInfo)
            {
                var subdir = new DirectoryInfo(i.FullName);
                var name = i.Name;
                if (name.CompareTo(DateTime.Now.ToString("yyyy-MM-dd")) < 0) //删除文件夹
                {
                    subdir.Delete(true);
                }
                else //删除文件
                {
                    System.IO.File.Delete(i.FullName);
                }
            }
        }
    }
    catch (Exception ex)
    {
        throw ex;
    }
}

/// <summary>
/// oracle储存过程
/// </summary>
private IList<Dictionary<string, string>> GetProDataInfo(ProDataModel model)
{
    if (!string.IsNullOrEmpty(model.BOID))
    {
        #region oracle 存储过程参数
        var Oparms = new OracleParameter[3];
        var oParam = new OracleParameter("P_DATACLASSID", OracleDbType.Int32);
        oParam.Direction = ParameterDirection.Input;
        oParam.Value = model.DATACLASSID;
        Oparms[0] = oParam;
        var oParam2 = new OracleParameter("P_BOID", OracleDbType.Varchar2);
        oParam2.Direction = ParameterDirection.Input;
        oParam2.Value = model.BOID;
        Oparms[1] = oParam2;
        var oParam3 = new OracleParameter("CSR_DATA", OracleDbType.RefCursor);
        oParam3.Direction = ParameterDirection.Output;
        Oparms[2] = oParam3;

        var cmd = context.Database.Connection.CreateCommand();
        cmd.CommandType = CommandType.StoredProcedure;
        cmd.CommandText = "GETOILFIELDDATA";
        cmd.Parameters.Clear();
        if (Oparms.Length > 0)
        {
            cmd.Parameters.AddRange(Oparms);
        }
        #endregion
        OracleDataReader reader = null;
        var records = new List<Dictionary<string, string>>();
        try
        {
            cmd.Connection.Open();
            cmd.ExecuteNonQuery();
            Oracle.ManagedDataAccess.Types.OracleRefCursor cur = Oparms[2].Value as Oracle.ManagedDataAccess.Types.OracleRefCursor;
            reader = cur.GetDataReader();
            var row = reader.GetSchemaTable().Rows;
            foreach (DbDataRecord obj in reader)
            {
                var record = new Dictionary<string, string>();
                object[] values = new object[obj.FieldCount];
                obj.GetValues(values);
                for (var i = 0; i < values.Length; i++)
                {
                    var key = row[i].Field<string>("ColumnName");
                    var value = values[i] == DBNull.Value ? "" : values[i].ToString();
                    record.Add(key, value);
                }
                records.Add(record);
            }
        }
        catch (Exception ex) { throw ex; }
        finally
        {
            if (reader != null)
            {
                reader.Close();
                reader.Dispose();
            }
            cmd.Connection.Close();
        }
        return records;
    }
    return null;
}

/// <summary>
/// 非ef join
/// </summary>
private IList<ProDataModel> GetProDataList(int dataClassId, string name)
{
    var query =
    from a in context.OILFIELDDATA
    join b in context.APPDATAMODEL on new { a.DATACLASSID, a.KEY } equals new { b.DATACLASSID, b.KEY }
    where a.DATACLASSID == dataClassId && a.BOID == name
    orderby b.KEYINDEX
    select new ProDataModel()
    {
        DATACLASSID = a.DATACLASSID,
        BOID = a.BOID,
        KEY = a.KEY,
        VALUE = a.VALUE,
        CAPTION = b.CAPTION,
        VALTYPE = b.VALFORMAT,
        VALFORMAT = b.VALFORMAT,
        DEFSHOW = b.DEFSHOW,
        KEYINDEX = b.KEYINDEX
    }; 
    var list = query.ToList(); 
    return list;
}

/// <summary>
/// 非ef union
/// </summary>
private IList<APPDATAPROFILE> GetAppDataProfileModel(decimal userId = 0)
{
    var appdataProfile = userContext.APPDATAPROFILE.ToList();
    var appdataModel = context.APPDATAMODEL.ToList(); 
    var query = (from user in appdataProfile
                 orderby user.DATACLASSID, user.KEYINDEX
                 select new APPDATAPROFILE
                 {
                     DATACLASSID = user.DATACLASSID,
                     KEY = user.KEY,
                     CAPTION = user.CAPTION,
                     VALTYPE = user.VALTYPE,
                     VALFORMAT = user.VALFORMAT,
                     KEYINDEX = user.KEYINDEX,
                     DEFSHOW = user.DEFSHOW,
                     CAPTIONEXTEND = user.CAPTIONEXTEND
                 }
                  ).Union(from sys in appdataModel
                          where !(from user2 in appdataProfile
                                  where user2.USERID == userId
                                  group user2 by user2.DATACLASSID
                          into g
                                  select g.Key).Contains(sys.DATACLASSID)
                          orderby sys.DATACLASSID, sys.KEYINDEX

                          select new APPDATAPROFILE
                          {
                              DATACLASSID = sys.DATACLASSID,
                              KEY = sys.KEY,
                              CAPTION = sys.CAPTION,
                              VALTYPE = sys.VALTYPE,
                              VALFORMAT = sys.VALFORMAT,
                              KEYINDEX = sys.KEYINDEX,
                              DEFSHOW = sys.DEFSHOW,
                              CAPTIONEXTEND = sys.CAPTIONEXTEND
                          }
                          );
    var list = query.ToList();
    return list;
}

        /// 生成流
public static Stream ToStream(this XmlDocument xdoc)
        {
            var memoryStream = new MemoryStream();
            xdoc.Save(memoryStream);
            return memoryStream;
        }