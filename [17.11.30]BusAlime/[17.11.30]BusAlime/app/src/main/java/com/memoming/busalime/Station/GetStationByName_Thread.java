package com.memoming.busalime.Station;

import com.memoming.busalime.Station.GetNextStation_Thread;
import com.memoming.busalime.Station.Station;

import org.xmlpull.v1.XmlPullParser;
import org.xmlpull.v1.XmlPullParserException;
import org.xmlpull.v1.XmlPullParserFactory;

import java.io.IOException;
import java.io.UnsupportedEncodingException;
import java.net.URL;
import java.net.URLEncoder;
import java.util.ArrayList;

/**
 * '면목' 이라고 검색하면 '면목'이 들어간 모든 정류장을 검색하여
 *  각각을 Station 객체에 저장하는 객체.
 */

public class GetStationByName_Thread extends Thread {

    /* 생성자 */
    public GetStationByName_Thread ( String stNm )  // stNm :: 정류소를 검색할 단어
            throws UnsupportedEncodingException {
        this.stNm = URLEncoder.encode(stNm, "UTF-8");
    }

    /* 기본 변수 선언 */
    private String                  stNm , _url;        // stNm은 정류소를 검색할 단어, _url은 공공데이터로 넘길 주소
    private Station station;            // 검색해서 여러 정류소가 나오면, 한개의 정류소를 한개의 station에 넣음
    private ArrayList<Station>      stations;           // 위에서 하나씩만든 station을 한번에 저장할 station형 ArrayList

    /* Parsing을 위한 변수 */
    private XmlPullParserFactory    factory;
    private XmlPullParser           parser;

    /* Thread 시작 */
    @Override
        public void run() {
        try {
            /* 변수 초기화 */
            stations    = new ArrayList<Station>();
            _url        = "http://ws.bus.go.kr/api/rest/stationinfo/getStationByName?ServiceKey=ahy2jeesQPj%2FU58va0GKMSMp9sK6LpX9sPhgW%2BJJyXD33sQr2s0xcPe7Az3HT1MH4XOo63DywC6RBVR8O1LUgQ%3D%3D&stSrch=";
            _url        = _url + stNm;                              // 실제 검색할 단어를 url에 파라미터로 붙임
            station     = null;                                     // 처음에는 station을 null로 초기화 해둠
            factory     = XmlPullParserFactory.newInstance();
            parser      = factory.newPullParser();
            URL url     = new URL(_url);                            // 만든 url String을 URL 객체에 넣음
            parser.setInput(url.openStream(), "UTF-8");             // URL 객체에서 읽어와 parser에 넣음

            int eventType = parser.getEventType();                  // XML Parser는 태그에따라 eventType 값이 바뀜

            while (eventType != XmlPullParser.END_DOCUMENT) {       // 문서의 끝이 아닌동안 while문 반복
                switch (eventType) {
                    case XmlPullParser.START_TAG:                   // eventType이 시작태그(<)인 경우
                        String tag = parser.getName();              // 해당 tag의 tag 이름을 가져와서

                        if (tag.compareTo("itemList") == 0 && station == null)          // 그 가져온 tag가 'itemList' 이면
                            station = new Station();                                    // 새로운 정류소가 시작되므로 station 객체를 초기화 해둠

                        else if (tag.compareTo("arsId") == 0 && station != null) {      // 그 가져온 tag가 'arsId' 이면
                            station.setArsId(parser.nextText().toString());             // 초기화 해둔 station 객체에 arsId를 set 해주고
                            GetNextStation_Thread getNextStationThread                  // 해당 정류소의 다음 정류소를 받아오려면 arsId가 필요하므로 여기에서 바로 처리해줌
                                    = new GetNextStation_Thread(station.getArsId());    // Thread를 생성하고 인자로 알아낸 arsId를 넘김
                            getNextStationThread.start();                               // 해당 Thread를 run 시킴
                            getNextStationThread.join();                                // 현재 실행되고 있는 Thread를 종료하지 않고 다음 Thread를 기다림
                            station.setNxtStn(getNextStationThread.getNxtStn());        // 실행 했던 Thread에서 nxtStn (다음정류소 이름)을 가져옴
                        }

                        else if (tag.compareTo("stNm") == 0 && station != null)         // 태그가 stNm (정류소명) 일경우
                            station.setStNm(parser.nextText());                         // station의 stNm값에 set 해줌

                        else if (tag.compareTo("tmX") == 0 && station != null)          // 태그가 tmX (경도) 일경우
                            station.setGpsY(parser.nextText());                         // station의 GpsY 값에 set 해줌

                        else if (tag.compareTo("tmY") == 0 && station != null) {        // 태그가 tmY (위도) 일경우
                            station.setGpsX(parser.nextText());                         // station의 GpsX 값에 set 해줌
                            stations.add(station);                                      // 실제 XML 결과 페이지에서 tmY는 하나의 정류소 맨끝에 나오는 값이므로
                            station = null;                                             // 여기에서 stations ArrayList에 추가하고 해당 station 객체를 초기화
                        }                                                               // 하여 다음 정류소 객체를 만들 준비를 함
                        break;
                }
                eventType = parser.next();
            }
        }
        catch (XmlPullParserException e)    { e.printStackTrace(); }
        catch (IOException e)               { e.printStackTrace(); }
        catch (InterruptedException e)      { e.printStackTrace(); }
    }

    public ArrayList<Station> getStations () {                                          // 해당 검색어로 검색된 모든 정류소가 들어있는 Station형 ArrayList를 반환
        return stations;
    }
}
