using UnityEngine;
using UnityEngine.UI;

public class HowToPlayController : MonoBehaviour
{
  public GameObject howToPlayPanel; // How to Play â
  public Image contentPanel; // �������� ������ �̹���
  public Button prevButton, nextButton; // �¿� ȭ��ǥ ��ư
  public Sprite[] pages; // ������ �̹�����

  private int currentPage = 0;

  void Start()
  {
    UpdatePage(); // ó�� ������ �� ������ ����
  }

  public void OpenHowToPlay()
  {
    howToPlayPanel.SetActive(true); // â ����
    currentPage = 0; // ù �������� ����
    UpdatePage();
  }

  public void CloseHowToPlay()
  {
    howToPlayPanel.SetActive(false); // â �ݱ�
  }

  public void NextPage()
  {
    if ( currentPage < pages.Length - 1 )
    {
      currentPage++;
      UpdatePage();
    }
  }

  public void PrevPage()
  {
    if ( currentPage > 0 )
    {
      currentPage--;
      UpdatePage();
    }
  }

  void UpdatePage()
  {
    //  contentPanel.sprite = pages[currentPage];

    // prevButton.gameObject.SetActive(currentPage > 0);


    // nextButton.gameObject.SetActive(currentPage < pages.Length - 1);
  }
}
