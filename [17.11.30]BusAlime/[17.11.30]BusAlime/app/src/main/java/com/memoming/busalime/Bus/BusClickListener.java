package com.memoming.busalime.Bus;

import android.content.Context;
import android.content.Intent;
import android.view.View;
import android.widget.AdapterView;

import com.memoming.busalime.Activity.StationListActivity;

public class BusClickListener implements AdapterView.OnItemClickListener {

    private Context currentContext;
    private BusItem busItem;

    @Override
    public void onItemClick(AdapterView<?> parent, View view, int position, long id) {

        currentContext = parent.getContext();
        busItem = (BusItem)( parent.getAdapter().getItem(position) );

        Intent intent = new Intent(currentContext, StationListActivity.class);
        intent.putExtra("busItem", busItem);
        currentContext.startActivity(intent);
    }

}
