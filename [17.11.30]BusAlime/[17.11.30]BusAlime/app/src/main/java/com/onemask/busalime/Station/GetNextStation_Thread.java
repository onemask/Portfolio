package com.onemask.busalime.Station;

import org.xmlpull.v1.XmlPullParser;
import org.xmlpull.v1.XmlPullParserException;
import org.xmlpull.v1.XmlPullParserFactory;
import java.io.IOException;
import java.net.MalformedURLException;
import java.net.URL; //빙고

/**
 * APP에서 정류장으로 검색했을때 어느 방면인지를 나타내 줘야 하기때문에
 * 다음 정류소를 알기 위해 여는 Thread.
 *
 * 정류장 고유 arsId를 넘겨주면 해당 정류소의 다음 정류소를
 * 구해서 return 해준다.
 */

public class GetNextStation_Thread extends Thread {

    /* 생성자 */
    public GetNextStation_Thread (String arsId) {
        this.arsId = arsId;
    }

    /* 기본 변수선언 */
    private String  arsId;   // 인자로 받아올 정류장 고유 arsId
    private String  nxtStn;  // 결과로 return할 다음 정류소명
    private String  _url;    // 공공데이터로 요청할 url
    private boolean flag;    // xmlParser가 태그를 돌다가 nxtStn을 만났을경우 true

    /* Parsing을 위한 변수 */
    private XmlPullParserFactory    factory;
    private XmlPullParser           parser;

    /* Android에서 웹에 요청할 URL형 변수 */
    private URL url;

    /* Thread 시작 */
    @Override
    public void run() {
        try {
            /* 변수 초기화 */
            _url    = "http://ws.bus.go.kr/api/rest/stationinfo/getStationByUid?serviceKey=O3pn45WEu%2FBqli3ighFdTaTWSEI%2F1ujcAfNF0e46w%2BTpHSyunMKjHnErWxFw9t%2Fzrb5htgLVe4CYySaHqJoYyQ%3D%3D&arsId=";
            flag    = false;
            factory = XmlPullParserFactory.newInstance();
            parser  = factory.newPullParser();
            url     = new URL(_url + arsId);                    // _url에 파라미터로 arsId를 더해서 공공데이터 검색
            parser.setInput(url.openStream(), "UTF-8");         // URL을 parser에 넣음

            int eventType = parser.getEventType();              // XML Parser는 태그에따라 eventType 값이 바뀜

            while (eventType != XmlPullParser.END_DOCUMENT) {   // 문서의 끝이 아닌동안 while문 반복
                switch (eventType) {
                    case XmlPullParser.START_TAG:               // eventType이 시작태그(<)인 경우
                        String tag = parser.getName();          // 해당 tag의 tag 이름을 가져와서
                        if (tag.compareTo("nxtStn") == 0) {     // 그 가져온 tag가 'nxtStn' 이면
                            flag = true;                        // flag를 true로 바뀌어서 while문을 빠져나갈 수 있도록
                            nxtStn = parser.nextText();         // nxtStn 변수에 해당 nxtStn 태그의 value값을 저장
                            }
                            break;
                    }
                    if (flag)                                   // flag true == 내가 원하던 nxtStn 값을 찾았음
                        break;                                  // while문 종료
                    eventType = parser.next();                  // 그것이 아니라면 다음 parser를 읽음
                }
            }

        catch (XmlPullParserException e)    { e.printStackTrace(); }
        catch (MalformedURLException e)     { e.printStackTrace(); }
        catch (IOException e)               { e.printStackTrace(); }
    }

    /* 결과를 return 해주는 함수 */
    /* nxtStn 이란 변수가 private이기 때문에 필요함 */
    public String getNxtStn() {
        return nxtStn;
    }
}


