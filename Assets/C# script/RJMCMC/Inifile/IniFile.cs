using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;

public class IniFile  
{
	// the full name of the file
	string fileFullName;

	// pair of name and value
	// eg. name = value ; comment
	class Pair
	{
		public string name;
		public string value;
		public string comment;

		public Pair(string n, string v, string c)
		{
			name = n;
			value = v;
			comment = c;
		}
	}

	// section
	class Section
	{
		public string name;
		public string comment;
		public Dictionary<string, Pair> pairs;

		public Section(string n, string c)
		{
			name = n;
			comment = c;
			pairs = new Dictionary<string, Pair>();
		}

	}
	Section current_section;

	// data
	Dictionary<string, Section> data;


	public IniFile()
	{
		current_section = null;
		data = new Dictionary<string, Section>();
	}

	public int Section_Count()
	{
		return data.Count;
	}

	// load a exist file, otherwise, return false
	public bool Load_File(string fileName)
	{

		fileFullName = fileName;

		// file NOT exist
		if (!System.IO.File.Exists(fileName))
			return false;


		StreamReader sr = new StreamReader(fileFullName);

		while (!sr.EndOfStream)
		{
			string line = sr.ReadLine();
			line = line.Trim();

			if (line.Length == 0)
				continue;

			if (Is_Load_Comment(line))
				continue;


			// if the first letter is "[", it is section 
			// eg. [MySection]
			if (Is_Load_A_Section(line))
			{
				Load_A_Section(line);
			}
			else
			{
				Load_A_Pair(line);
			}

		}

		// end load
		sr.Close();
		current_section = null;

		return true;
	}

	bool Is_Load_Comment( string rawLine)
	{
		if (rawLine[0] == ';')
		{
			return true;
		}
		return false;
	}

	bool Is_Load_A_Section(string rawLine)
	{
		if (rawLine[0] == '[')
		{
			return true;
		}
		return false;
	}

	void Load_A_Section(string rawLine)
	{
       // Debug.Log(rawLine);
		// read the section name and comment
		int startSection = rawLine.IndexOf('[');
		int endSection = rawLine.LastIndexOf(']');
		string name = rawLine.Substring((startSection + 1), (endSection - startSection - 1));
		string comment = "";
		if (rawLine.Split(';').Length >= 2)
			comment = rawLine.Split(';')[1];

		name = name.Trim();
		comment = comment.Trim();

		current_section = new Section(name, comment);
		data.Add(name,current_section);
	}

	void Load_A_Pair(string rawLine)
	{
        //Debug.LogError(rawLine);
		// read the pair name, value and comment
		string name = rawLine.Split('=')[0];
		string value = rawLine.Split('=')[1].Split(';')[0];
		string comment = "";
		if (rawLine.Split('=')[1].Split(';').Length >= 2)
			comment = rawLine.Split('=')[1].Split(';')[1];

		name = name.Trim();
		value = value.Trim();
		comment = comment.Trim();

		Pair p = new Pair(name, value, comment);
       // Debug.LogError("name" + name + "value" +value+"comment"+ comment);
		current_section.pairs.Add(name, p);
	}


	// is section exist
	public bool Is_Section(string section)
	{
		return data.ContainsKey(section);
	}

	// if section not exist, return false
	public bool Goto_Section(string section)
	{
		if (Is_Section(section))
		{
			current_section = data[section];
			return true;
		}

		return false;
	}

	// create section, if exist, update the comment
	public bool Create_Section(string section, string comment = "")
	{
		if (Is_Section(section))
		{
			current_section = data[section];
			current_section.comment = comment;
			return false;
		}

		current_section = new Section(section, comment);
		data.Add(section, current_section);
		
		return true;
	}

	public bool Is_Name(string name)
	{
		if (current_section != null)
		{
			return current_section.pairs.ContainsKey(name);
		}

		return false;
	}

	public string Get_Section_Comment()
	{
		if (current_section != null)
		{
			return current_section.comment;
		}
		
		return "";
	}
	
	// get comment
	public string Get_Comment(string name, string defaultValue = "")
	{
		if (current_section != null)
		{
			if (Is_Name(name))
			{
				return current_section.pairs[name].comment;
			}
		}
		
		return defaultValue;
	}

	// get string
	public string Get_String(string name, string defaultValue = "")
	{
		if (current_section != null)
		{
			if (Is_Name(name))
			{
				return current_section.pairs[name].value;
			}
		}

		return defaultValue;
	}

    public string[] Get_Strings(string name, string defaultValue = "")
    {
        string strConn = Get_String(name,defaultValue);
        string[] subs = strConn.Split(',');
        for (int i=0; i<subs.Length; i++)
        {
            subs[i] = subs[i].Trim();
        }
        return subs;
    }

	// get bool
	public bool Get_Bool(string name, bool defaultValue = false)
	{
		return bool.Parse (Get_String (name, defaultValue.ToString ()));
	}

	// get int
	public int Get_Int(string name, int defaultValue = 0)
	{
		return int.Parse(Get_String(name, defaultValue + ""));
	}

	// get float
	public float Get_Float(string name, float defaultValue = 0f)
	{
		return float.Parse(Get_String(name, defaultValue + ""));
	}

	// get color
//	public Color Get_Color(string name, Color defaultValue)
//	{
//		Color c = defaultValue;
//		Color.TryParseHexString(Get_String(name,c.ToHexStringRGBA()), out c);
//		return c;
//	}
//

	// get vector
	public Vector3 Get_Vector3(string name, Vector3 defaultValue)
	{
		Vector3 v = defaultValue;
		string s = Get_String (name, "");
		if (s == "")
			return v;
		
		string[] ss = s.Split (',');
		v.x = float.Parse(ss [0]);
		v.y = float.Parse(ss [1]);
		v.z = float.Parse(ss [2]);

		return v;
	}

	public Quaternion Get_Quaternion(string name, Quaternion defaultValue)
	{
		Quaternion q = defaultValue;
		string s = Get_String (name, "");
		if (s == "")
			return q;

		string[] ss = s.Split (',');
		q.x = float.Parse(ss [0]);
		q.y = float.Parse(ss [1]);
		q.z = float.Parse(ss [2]);
		q.w = float.Parse(ss [3]);
		return q;
	}

    public Color Get_Color(string name, Color defaultValue)
    {
        Color c = defaultValue;
        string s = Get_String(name, "");
        if (s == "")
            return c;

        string[] ss = s.Split(',');
        c.r = float.Parse(ss[0]);
        c.g = float.Parse(ss[1]);
        c.b = float.Parse(ss[2]);
        c.a = float.Parse(ss[3]);
        return c;
    }

    public float[] Get_FloatArray(string name, float[] defaultValue)
	{
		string s = Get_String (name, "");
		if (s == "")
			return defaultValue;

		string[] ss = s.Split (',');
		float[] f = new float[ss.Length];
		for (int i = 0; i < f.Length; i++) {
			f [i] = float.Parse (ss [i]);
		}

		return f;
	}

	public void Set_FloatArray(string name, float[] f, string comment ="")
	{
		string s ="";
		for (int i=0; i<f.Length-1; i++)
		{
			s +=(f[i]+",");
		}
		if (f.Length>0)
		{
			s +=(f[f.Length-1]+"");
		}



		Set_String(name,s,comment);
	}

    public int[] Get_IntArray(string name, int[] defaultValue)
    {
        string s = Get_String(name, "");
        if (s == "")
            return defaultValue;

        string[] ss = s.Split(',');
        int[] f = new int[ss.Length];
        for (int i = 0; i < f.Length; i++)
        {
            f[i] = int.Parse(ss[i]);
        }

        return f;
    }

    public void Set_IntArray(string name, int[] f, string comment = "")
    {
        string s = "";
        for (int i = 0; i < f.Length - 1; i++)
        {
            s += (f[i] + ",");
        }
        if (f.Length > 0)
        {
            s += (f[f.Length - 1] + "");
        }
        

        Set_String(name, s, comment);
    }

    // set string or update
    public void Set_String(string name, string value, string comment = "")
	{
		if (current_section != null)
		{
			if (Is_Name(name))
			{
				current_section.pairs[name].value = value;
				if (comment != "")
					current_section.pairs[name].comment = comment;
			}
			else
			{
				current_section.pairs.Add(name, new Pair(name, value, comment));
			}
		}
	}

	// set bool or update
	public void Set_Bool(string name, bool value, string comment = "")
	{
		Set_String (name, value.ToString (), comment);
	}

	// set int or update
	public void Set_Int(string name, int value, string comment = "")
	{
		Set_String(name, value + "", comment);
	}

	// set float or update
	public void Set_Float(string name, float value, string comment = "")
	{
		Set_String(name, value + "", comment);
	}

	public void Set_Vector3(string name, Vector3 v, string comment = "")
	{
		string s = "" + v.x + "," + v.y + "," + v.z;
		Set_String(name, s, comment);
	}

	public void Set_Quaternion(string name, Quaternion q, string comment = "")
	{
		string s = "" + q.x + "," + q.y + "," + q.z + ","+q.w;
		Set_String(name, s, comment);
	}



    // set Color
    public void Set_Color(string name, Color value, string comment = "")
    {
        string s = "" + value.r + "," + value.g + "," + value.b + "," + value.a;
        Set_String(name, s, comment);
    }
    //
    public void Save()
	{
		SaveTo(fileFullName);
	}

	public void SaveTo(string fileName)
	{
		fileFullName = fileName;

		StreamWriter sw = new StreamWriter(fileName,false);

		foreach(string key in data.Keys)
		{
			Section s = data[key];
			sw.WriteLine("[" + s.name + "]" + ((s.comment == "") ? "" : (" ; " + s.comment)));
			foreach(string name in s.pairs.Keys)
			{
				Pair p = s.pairs[name];
				sw.WriteLine( "  " + p.name + " = " + p.value + ((p.comment == "") ? "" : (" ; " + p.comment)));
			}
		}
		sw.Close();
	}

	public string[] Get_All_Section()
	{
		List<string> ret = new List<string>();
		foreach(string key in data.Keys)
		{
			ret.Add(key);
		}
		return ret.ToArray();
	}

	public string[] Get_All_Pair_Names()
	{
		List<string> ret = new List<string>();
		foreach(string key in current_section.pairs.Keys)
		{
			ret.Add(key);
		}
		return ret.ToArray(); 
	}

	public string[] Get_All_Pair_Values()
	{
		List<string> ret = new List<string>();
		foreach(string key in current_section.pairs.Keys)
		{
			ret.Add(current_section.pairs[key].value);
		}
		return ret.ToArray(); 
	}

	public void Remove_Section(string section)
	{
		data.Remove (section);
	}




}
















