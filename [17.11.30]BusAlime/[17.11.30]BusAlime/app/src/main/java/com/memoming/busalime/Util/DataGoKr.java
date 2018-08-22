package com.memoming.busalime.Util;

public class DataGoKr {
    private String serviceKey;
    private String url_getBusList_ByBusNumber;
    private String url_getStatinList_ByStationName;
    private String url_getStationList_ByBusRouteId;

    public DataGoKr () {
        serviceKey
                = "O3pn45WEu%2FBqli3ighFdTaTWSEI%2F1ujcAfNF0e46w%2BTpHSyunMKjHnErWxFw9t%2Fzrb5htgLVe4CYySaHqJoYyQ%3D%3D";
        url_getBusList_ByBusNumber
                = "http://ws.bus.go.kr/api/rest/busRouteInfo/getBusRouteList?";
        url_getStatinList_ByStationName
                = "http://ws.bus.go.kr/api/rest/stationinfo/getStationByName?";
        url_getStationList_ByBusRouteId
                = "http://ws.bus.go.kr/api/rest/busRouteInfo/getStaionByRoute?";

    }

    public String getServiceKey () {
        return serviceKey;
    }
    public String getUrl_getBusList_ByBusNumber         () {
        return url_getBusList_ByBusNumber+"serviceKey="+serviceKey;
    }
    public String getUrl_getStatinList_ByStationName    () {
        return url_getStatinList_ByStationName+"serviceKey="+serviceKey;
    }
    public String getUrl_getStationList_ByBusRouteId    () {
        return url_getStationList_ByBusRouteId+"serviceKey="+serviceKey;
    }
}
