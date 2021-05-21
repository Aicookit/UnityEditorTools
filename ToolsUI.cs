using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;

public class ToolsUI : EditorWindow
{
    //[MenuItem("ToolsUI/ToolKit", false, 1)]
    public static void ShowToolWnd(List<EngineTeam.Package.PackageData> list)
    {
        // 获取现有打开的窗口；如果没有，则新建一个窗口：
        //EditorWindow ppp = GetWindow(typeof(ToolsUI), true, "工具", true);
        ToolsUI PMgrWindow = GetWindow<ToolsUI>(true, "工具管理", true);
        PMgrWindow.m_List = list;
        PMgrWindow.Show();

        //EngineTeam.Package.Utility.DownloadCOmpletedCallback = null;
        //EngineTeam.Package.Utility.DownloadCOmpletedCallback += () => { Debug.LogError("Repaint"); PMgrWindow.OnGUI(); };
    }

    List<EngineTeam.Package.PackageData> m_List;

    public bool[] foldoutState = new bool[8]; //最多8个版本
    public Vector2 scrollPosition;            // scroll滑动位置

    string currentAccount = LoginToolUI.inputAcount; //服务端的账号连接
    string currentPackage;
    string isInstalledVersion;
    string selectVersion;
    string latestVersion;
    string description; // package的描述

    string InstallBtnName = "Install";
    string RemoveBtnName = "Remove";
    bool InstallBtn=false;                // 默认false
    bool RemoveBtn;


    int bgColori = -1;
    int bgColorj = -1;

    public void ShowPackagesList()
    {
        //自定义风格
        //GUIStyle btnStyle = new GUIStyle("install");
        //btnStyle.active = false;
        //GUI.color = Color.red;
        //GUIStyle fontStyle = new GUIStyle();
        //fontStyle.alignment = TextAnchor.MiddleCenter;
        //fontStyle.fontSize = 9;

        scrollPosition = GUI.BeginScrollView(new Rect(0, 0, position.width / 3, position.height), scrollPosition, new Rect(0, 0, position.width / 2.8f, position.height / 0.8f));//(自身的位置，滚动条方位，背景位置设置）
        GUILayout.BeginArea(new Rect(0, 0, position.width / 3, position.height)); // (x起点,y起点，窗口x方向占宽比例，y方向展宽

        GUISkin skin = GUI.skin;
        skin.button.hover.textColor = Color.gray;
        GUI.skin.button.wordWrap = true;

        for (int i = 0; i < m_List.Count; i++)
        {
            foldoutState[i] = EditorGUILayout.Foldout(foldoutState[i], $"{m_List[i].package}                                                {selectVersion}", true);
            //foldoutState[i] = EditorGUILayout.Foldout(foldoutState[i], $"{m_List[i].package}", true);
            if (foldoutState[i])
            {
                for (int j = 0; j < m_List[i].versionlist.Length; j++)  // 获取插件下版本数量
                {

                    if (bgColori == i && bgColorj == j)
                    {
                        GUI.backgroundColor = Color.gray;
                    }

                    if (GUILayout.Button($"版本{m_List[i].versionlist[j]}", GUILayout.Width(position.width / 3))) // 选择当前版本
                    {
                        bgColori = i;
                        bgColorj = j;
                        currentPackage = m_List[i].package;   // 当前选择版本的插件名
                        selectVersion = m_List[i].versionlist[j]; //当前所选版本
                        description = m_List[i].description; // package插件描述
                     
                        if (EngineTeam.Package.Utility.IsInstalled(currentPackage,m_List[i].versionlist[j]))  // 如果当前包中已有version 被安装
                        {
                            isInstalledVersion = m_List[i].versionlist[j];
                            Debug.Log($"当前所选包有{m_List[i].versionlist.Length}个版本");
                            ShowNotification(new GUIContent($"当前所选插件中有{m_List[i].versionlist.Length}个版本,其中版本{isInstalledVersion}已安装"));
                        }


                        #region 更新到当前选择版本
                        //
                        if (m_List[i].versionlist.Length > 1)  // 按钮激活不激活不是由选择时的按钮判断，而是由是否存在的版本决定
                        {
                            Debug.Log($"本插件共有{m_List[i].versionlist.Length}个版本");// 在多版本中，判断是否包中有版本是被安装了
                            Debug.Log($"当前选择了插件{m_List[i].package}中的版本{m_List[i].versionlist[j]}");
                           

                            if ((EngineTeam.Package.Utility.IsInstalled(currentPackage, isInstalledVersion))) // 如果本地已存在其他版本 ，就update
                            {
                                if (EngineTeam.Package.Utility.IsInstalled(currentPackage, selectVersion)) // 当前所选版本已安装
                                {
                                    Debug.Log($"当前选中的版本{selectVersion}已经安装了");
                                    ShowNotification(new GUIContent($"当前所选版本{selectVersion}已被安装"));
                                    InstallBtn = false;
                                    InstallBtnName = "Installed";
                                    RemoveBtn = true;
                                }
                                else  // 当前所选版本未安装
                                {
                                    Debug.LogError($"当前插件中已有{isInstalledVersion}版本被安装！, 是否更新到当前所选版本{selectVersion}？");
                                   
                                    InstallBtn = true;
                                    InstallBtnName = $"Update to {selectVersion}";
                                    RemoveBtn = false;
                                }
                            }

                            else   // 不存在任何版本
                            {
                                InstallBtn = true;
                                InstallBtnName = "install";
                                RemoveBtn = false;

                            }
                        }
                        //当前所选版本数量小于等于1时  m_List[i].versionlist.Length <= 1
                        else
                        {
                            if (!EngineTeam.Package.Utility.IsInstalled(currentPackage, selectVersion))
                            {
                                Debug.Log($"本插件有{m_List[i].versionlist.Length}个版本,是否安装{selectVersion}");
                                ShowNotification(new GUIContent($"本插件有{m_List[i].versionlist.Length}个版本,是否安装{selectVersion}"));

                                InstallBtn = true;
                                InstallBtnName = "install";
                                RemoveBtn = false;
                            }
                            else
                            {
                                Debug.Log($"已经安装了版本{selectVersion}");
                                InstallBtn = false;
                                InstallBtnName = "Installed";
                                RemoveBtn = true;
                            }
                        }


                        //
                        //if (m_List[i].versionlist.Length > 1)
                        //{
                        //    Debug.Log($"本插件共有{m_List[i].versionlist.Length}个版本"); // 在多版本中，怎么判断是否安装包中有版本是被安装了？

                        //    if (EngineTeam.Package.Utility.IsInstalled(currentPackage, selectVersion))  // 按钮激活不激活不是由选择时的按钮判断，而是由是否存在的版本决定
                        //    {
                        //        Debug.Log($"已经安装了版本{selectVersion}");
                        //        InstallBtn = false;
                        //        InstallBtnName = "Installed";
                        //        RemoveBtn = true;
                        //    }
                        //    else
                        //    {
                        //         // 么有安装此版本
                        //        InstallBtn = true;
                        //        InstallBtnName = $"Update to {selectVersion}";
                        //        RemoveBtn = false;
                        //    }
                        //}
                        ////当前所选版本数量小于等于1时  m_List[i].versionlist.Length <= 1
                        //else
                        //{
                        //    if (!EngineTeam.Package.Utility.IsInstalled(currentPackage, selectVersion))
                        //    {
                        //        Debug.Log($"本插件有{m_List[i].versionlist.Length}个版本,是否安装{selectVersion}");
                        //        ShowNotification(new GUIContent($"本插件有{m_List[i].versionlist.Length}个版本,是否安装{selectVersion}"));
                        //        InstallBtn = true;
                        //        InstallBtnName = "install";
                        //        RemoveBtn = false;
                        //    }
                        //    else
                        //    {
                        //        Debug.Log($"已经安装了版本{selectVersion}");
                        //        InstallBtn = false;
                        //        InstallBtnName = "Installed";
                        //        RemoveBtn = true;
                        //    }
                        //}
                        #endregion

                        #region 自定义选择版本
                        //if (!EngineTeam.Package.Utility.IsInstalled(currentPackage, selectVersion))
                        //{

                        //    Debug.Log($"本地未安装此版本{selectVersion},是否安装？");
                        //    ShowNotification(new GUIContent($"本地未安装此版本{selectVersion},是否安装？"));
                        //    InstallBtn = true;
                        //    RemoveBtn = false;
                        //    InstallBtnName = "install";
                        //}
                        //else
                        //{
                        //    Debug.Log($"已经安装了版本{selectVersion}");
                        //    ShowNotification(new GUIContent($"已经安装了版本{selectVersion}"));
                        //    InstallBtn = false;
                        //    InstallBtnName = "Installed";
                        //    RemoveBtn = true; // btn可用
                        //}
                        #endregion
                    }
                    GUI.backgroundColor = Color.white;
                }
            }
        }
 

        GUI.skin = skin;
        GUILayout.EndArea();
        GUI.EndScrollView();

        //Right Area
        GUILayout.BeginArea(new Rect(position.width / 3, 0, position.width, position.height));//right detail
        GUILayout.BeginHorizontal();

        GUILayout.Space(position.width / 2.3f);

        //切换账号
        GUI.skin.button.wordWrap = true;
        if (GUILayout.Button($"切换账号：{currentAccount}", GUILayout.Width(0)))
        {
            this.Close();          // 退出后，需要与服务器断联
            EditorGUILayout.HelpBox("退出当前登陆", MessageType.Info); // 显示一个提示框
            LoginToolUI.ShowLGWnd();
        }

        GUILayout.EndHorizontal();
        GUILayout.Label(description, new GUIStyle(EditorStyles.wordWrappedLabel) { fontSize = 18 }); //绘制版本详细信息
        GUILayout.BeginHorizontal();

        GUI.enabled = InstallBtn;  // 控制 InstallBtn 安装按钮 是否可用
        GUI.skin.button.wordWrap = true;
        if (GUILayout.Button(InstallBtnName, GUILayout.Width(0)))  // install向服务器请求安装包 当版本 安装完成之后 且 本地存在此版本 ，InstallBtn要被置灰
        {

            EngineTeam.Package.Utility.RequestPackage(currentAccount, currentPackage, selectVersion);  // 点击安装后 需要再次 刷新 渲染窗口中的 插件按钮 状态
            Debug.Log($"正在安装{selectVersion}...");

            //if (!EngineTeam.Package.Utility.IsInstalled(currentPackage, selectVersion))
            //{
            //    InstallBtn = true;
            //    InstallBtnName = "install";
            //    RemoveBtn = false;

            //    EngineTeam.Package.Utility.RequestPackage(currentAccount, currentPackage, selectVersion);  // 点击安装后 需要再次 刷新 渲染窗口中的 插件按钮 状态
            //    Debug.Log($"正在安装{selectVersion}...");
            //}
            //else
            //{
            //    InstallBtn = false;
            //    InstallBtnName = "Installed";
            //    RemoveBtn = true;
            //    Debug.Log($"已经安装了版本{selectVersion}");
            //}
        }

        GUI.enabled = RemoveBtn;
        GUI.skin.button.wordWrap = true;
        if (GUILayout.Button(RemoveBtnName, GUILayout.Width(0)))   // remove
        {

            EngineTeam.Package.Utility.DeletePackage(currentPackage); //delete 移除本地安装包
            Debug.Log("正在删除版本{selectVersion}");

            //if (EngineTeam.Package.Utility.IsInstalled(currentPackage, selectVersion))  // 有
            //{
            //    Debug.Log("正在删除版本{selectVersion}");
            //    EngineTeam.Package.Utility.DeletePackage(currentPackage); //delete 移除本地安装包
            //    InstallBtn = false;
            //    InstallBtnName = "Installed";
            //    RemoveBtn = true;
            //}

            //else
            //{
            //    Debug.Log($"本地不存在插件{selectVersion}!");
            //    ShowNotification(new GUIContent($"本地不存在插件{selectVersion}"));
            //    InstallBtnName = "install";
            //    RemoveBtn = false;
            //}
     
        }
        GUILayout.EndHorizontal();
        GUILayout.EndArea();
    }

    public void OnGUI()
    {
        ShowPackagesList();
    }
}


/// <summary>
/// 登录窗口
/// </summary>
public class LoginToolUI : EditorWindow
{
    public static string inputAcount;
    string inputPSW;

    [MenuItem("ToolsUI/LoginToolKit", false, 0)]
    public static void ShowLGWnd()
    {
        // 获取现有打开的窗口；如果没有，则新建一个窗口：
        //var LGwindow = GetWindowWithRect<LoginToolUI>(new Rect(0, 0, 500, 300));
        var LGwindow = GetWindow(typeof(LoginToolUI), false, "Login", true);
        LGwindow.Show();
    }

    #region 文件写入

    //static void Createfile(string path, string name, string inputAcount)
    //{
    //    StreamWriter sw;//流信息
    //    FileInfo t = new FileInfo(path + "//" + name);
    //    if (!t.Exists)  //判断文件是否存在
    //    {
    //        sw = t.CreateText();//不存在，创建
    //    }
    //    else
    //    {
    //        sw = t.AppendText();//存在，则打开
    //    }
    //    sw.WriteLine(inputAcount);//以行的形式写入信息
    //    sw.Close();//关闭流
    //    sw.Dispose();//销毁流
    //}
    #endregion

    //显示登录UI
    void LoginWin()
    {

        GUILayout.Space(50);

        //GUILayout.BeginHorizontal();
        //inputAcount = EditorGUILayout.TextField("账         号", inputAcount);//通过后面GUILayoutOption参数自定义控件de大小
        //GUILayout.EndHorizontal();

        //GUILayout.BeginHorizontal();
        //inputPSW = EditorGUILayout.PasswordField("密        码:", inputPSW);
        //GUILayout.EndHorizontal();
        //if (GUILayout.Button("保存账号"))
        //{
        //    Createfile(Application.dataPath, "Asset/AccountFile.txt", inputAcount);
        //}
        //if (GUILayout.Button("清空", GUILayout.Width(550), GUILayout.Height(40)))
        //{
        //    inputAcount = "";
        //    inputPSW = "";
        //    ShowNotification(new GUIContent("已清空"));
        //}

        GUIStyle myStyle = new GUIStyle();
        myStyle.fontSize = 22;
        myStyle.normal.textColor = Color.white;
       
        GUILayout.BeginVertical();
     
        GUILayout.BeginHorizontal();
        GUILayout.Space(30);
        GUILayout.Label("账     号", myStyle, GUILayout.Width(40), GUILayout.Height(30));
        GUILayout.Space(50);
        inputAcount = GUILayout.TextField(inputAcount, GUILayout.Width(550), GUILayout.Height(30));
        GUILayout.EndHorizontal();
        GUILayout.Space(5);
        GUILayout.BeginHorizontal();
        GUILayout.Space(30);
        GUILayout.Label("密     码", myStyle, GUILayout.Width(40), GUILayout.Height(30));
        GUILayout.Space(50);
        inputPSW = GUILayout.TextField(inputPSW, GUILayout.Width(550), GUILayout.Height(30));
        GUILayout.EndHorizontal();

        GUILayout.Space(20);

        GUILayout.BeginHorizontal();
        GUILayout.Space(150);
        if (GUILayout.Button("退出", GUILayout.Width(position.width / 5), GUILayout.Height(40))) //关闭窗口
        {
            this.Close();
        }

        GUILayout.Space(100);
        if (GUILayout.Button("确认", GUILayout.Width(position.width / 5), GUILayout.Height(40)))
        {
            Action<List<EngineTeam.Package.PackageData>> completedAction = (list) =>
            {
                this.Close();
                ToolsUI.ShowToolWnd(list);//登陆成功后 跳转
            };

            if (inputAcount != null) // 输入不为空且与属于服务器中保存的账号
            {
                EngineTeam.Package.Utility.Login(inputAcount, completedAction);
                ShowNotification(new GUIContent("登陆成功"));
            }
            ShowNotification(new GUIContent("账号或密码错误,请重新输入!"));
        }
        GUILayout.EndHorizontal();
       
        GUILayout.EndVertical();
    }

    private void OnGUI()
    {
        LoginWin();
    }
}




