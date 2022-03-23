using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Menu : MonoBehaviour
{
   
    public void Menu_LoadScene_index(int sceneIndex)
    {
        Debug.Log("<Menu>\t\t Loading scene with index " + sceneIndex);

        /** ********************** **\
        \      SWITCHING SCENES      /
         |**************************|
        / Refer to Build Settings to \
        | to set & see scene indices |
        \** ********************** **/

        SceneManager.LoadScene(sceneIndex);

    } 

    public void Menu_LoadScene_name(string sceneName)
    {
        Debug.Log("<Menu>\t\t Loading scene "+ sceneName);

        /** ********************** **\
        \      SWITCHING SCENES      /
         |**************************|
        / All scenes still need to   \
        | be in build settings, but  |
        | using strings to load them |
        | instead of indices means   |
        | you can change their order |
        | without changing any code  |
        \** ********************** **/

        SceneManager.LoadScene(sceneName);
    }


    public static void Menu_Quit()
    {
        Debug.Log("<Menu>\t\t Quit called");
        Application.Quit();
    }

}
