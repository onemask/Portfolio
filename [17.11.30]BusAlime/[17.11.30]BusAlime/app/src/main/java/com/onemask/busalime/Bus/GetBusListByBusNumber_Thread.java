package com.onemask.busalime.Bus;

import com.onemask.busalime.Util.DataGoKr;

import org.xmlpull.v1.XmlPullParser;
import org.xmlpull.v1.XmlPullParserException;
import org.xmlpull.v1.XmlPullParserFactory;

import java.io.IOException;
import java.net.MalformedURLException;
import java.net.URL;
import java.util.ArrayList;


public class GetBusListByBusNumber_Thread extends Thread {

    /* 변수 선언 */
    private DataGoKr             dataGoKr;
    private String               param;
    private String               requestURL;
    private URL                  url;
    private BusItem              busItem;
    private ArrayList<BusItem>   busList;
    private int                  eventType;
    private XmlPullParserFactory factory;
    private XmlPullParser        parser;

    public GetBusListByBusNumber_Thread(String searchBusNumber) {
        dataGoKr    = new DataGoKr();
        busList     = new ArrayList<BusItem>();
        param       = "&strSrch=" + searchBusNumber;
        requestURL  = dataGoKr.getUrl_getBusList_ByBusNumber() + param;
    }

    @Override
    public void run() {
        try {
            busItem     = null;
            factory     = XmlPullParserFactory.newInstance();
            parser      = factory.newPullParser();
            url         = new URL( requestURL );
            parser.setInput(url.openStream(), "UTF-8");
            eventType   = parser.getEventType();

            while (eventType != XmlPullParser.END_DOCUMENT) {
                switch (eventType) {
                    case XmlPullParser.START_TAG:
                        String tag = parser.getName();

                        if (tag.compareTo("itemList") == 0 && busItem == null)
                            busItem = new BusItem();

                        else if (tag.compareTo("busRouteId") == 0) {
                            busItem.setRouteId(parser.nextText());
                        }

                        else if (tag.compareTo("busRouteNm") == 0)
                            busItem.setBusNm(parser.nextText());

                        else if (tag.compareTo("edStationNm") == 0)
                            busItem.setEnStationNm(parser.nextText());

                        else if (tag.compareTo("routeType") == 0)
                            busItem.setBusType(parser.nextText());

                        else if (tag.compareTo("stStationNm") ==0) {
                            busItem.setStStationNm(parser.nextText());
                            busList.add(busItem);
                            busItem = null;
                        }
                        break;
                } // switch end
                eventType = parser.next();
            } // while end
        }
        catch (XmlPullParserException e)    { e.printStackTrace(); }
        catch (MalformedURLException e)     { e.printStackTrace(); }
        catch (IOException e)               { e.printStackTrace(); }
    } // run end

    public ArrayList<BusItem> getBusList () { return busList; }

} // class end
