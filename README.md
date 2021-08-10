# WebFormParser
The WebFormParser repository contains two tools:
* **ASPXParser**: Parses Web Forms control usage
* **ASPXAggregator**: Aggregates multiple output files from the ASPXParser

## ASPXParser
Parses all asp controls and user controls across all aspx, ascx, master pages, and lists them in the order of use. It also searches the respective code behind files for events that are directly registered in the codebehinds and lists them too.
For each control, all attributes (across all pages for same controls) are listed along with server side events.
The output is presented in 2 formats, json format (saved to running directory) and normal console output.

**Console Output**

```
Solution-Level Data:
-- Projects: 1
-- WebFormsViewFiles: 7
-- WebFormsCodeBehindFiles: 7
-- UserControlFiles: 1
-- UserControlCodeBehindFiles: 1
-- MasterFiles: 2
-- MasterFileCodeBehindFiles: 2
-- TotalWebFormsFiles: 20

Controls Data:
-- Name :asp:label, Type: Asp, Count: 1697
  |-- Attribute:enabletheming, Count:18
  |-- Attribute:text, Count:836
  |-- Attribute:visible, Count:166
  |-- Attribute:font-bold, Count:64
  |-- Attribute:forecolor, Count:64
  |-- Attribute:font-names, Count:36
  |-- Attribute:font-size, Count:56
  |-- Attribute:style, Count:32
  |-- Attribute:height, Count:12
  |-- Attribute:cssclass, Count:101
  |-- Attribute:tooltip, Count:26
  |-- Attribute:class, Count:12
  |-- Attribute:backcolor, Count:8
-- Name :asp:hiddenfield, Type: Asp, Count: 1454
  |-- Attribute:value, Count:6
-- Name :asp:templatefield, Type: Asp, Count: 750
  |-- Attribute:headertext, Count:582
  |-- Attribute:headerstyle-horizontalalign, Count:22
  |-- Attribute:visible, Count:132
  |-- Attribute:sortexpression, Count:302
  |-- Attribute:showheader, Count:30
  |-- Attribute:insertvisible, Count:4
  |-- Attribute:itemstyle-horizontalalign, Count:4
  |-- Attribute:itemstyle-wrap, Count:2
  |-- Attribute:headerstyle-cssclass, Count:4
  |-- Attribute:itemstyle-cssclass, Count:4

```
**JSON Output**
```
{
    "Projects": 1,
    "WebFormsViewFiles": 7,
    "WebFormsCodeBehindFiles": 7,
    "UserControlFiles": 1,
    "UserControlCodeBehindFiles": 1,
    "MasterFiles": 2,
    "MasterFileCodeBehindFiles": 2,
    "TotalWebFormsFiles": 20
}

[
  {
 
    ControlName:"asp:label",
    NumberOfOccurences:1697,
    Attributes:{
 
      enabletheming:18,
      text:836,
      visible:166,
      'font-bold':64,
      forecolor:64,
      'font-names':36,
      'font-size':56,
      style:32,
      height:12,
      cssclass:101,
      tooltip:26,
      class:12,
      backcolor:8
    }
  },
  {
 
    ControlName:"asp:hiddenfield",
    NumberOfOccurences:1454,
    Attributes:{
 
      value:6
    }
  }
```

## ASPXAggregator
Parses all output files from the ASPXParser project in a given directory and aggregates the total usage.

The output is presented in 2 formats, json format (saved to running directory) and normal console output.


**Example Input 1**
```
[
    {
        "ControlName": "asp:label",
        "NumberOfOccurrences": 19,
        "Attributes":
        {
            "text": 18,
            "cssclass": 2
        }
    },
    {
        "ControlName": "asp:scriptreference",
        "NumberOfOccurrences": 13,
        "Attributes":
        {
            "name": 13
        }
    },
    {
        "ControlName": "asp:textbox",
        "NumberOfOccurrences": 13,
        "Attributes":
        {
            "cssclass": 13,
            "text": 7,
            "readonly": 1,
            "tooltip": 1
        }
    }
]
```

**Example Input 2**
```
[
    {
        "ControlName": "asp:label",
        "NumberOfOccurrences": 19,
        "Attributes":
        {
            "text": 18,
            "cssclass": 2
        }
    },
    {
        "ControlName": "asp:scriptreference",
        "NumberOfOccurrences": 13,
        "Attributes":
        {
            "name": 13,
            "assembly": 8,
            "path": 8
        }
    }
]
```

**Console Output**

Controls Data:
-- Name :asp:label, Type: Html, Count: 38
  |-- Attribute:text, Count:36
  |-- Attribute:cssclass, Count:4
-- Name :asp:scriptreference, Type: Html, Count: 26
  |-- Attribute:name, Count:26
  |-- Attribute:assembly, Count:8
  |-- Attribute:path, Count:8
-- Name :asp:textbox, Type: Html, Count: 13
  |-- Attribute:cssclass, Count:13
  |-- Attribute:text, Count:7
  |-- Attribute:readonly, Count:1
  |-- Attribute:tooltip, Count:1
