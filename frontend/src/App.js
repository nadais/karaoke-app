import React, { useState, useEffect } from 'react';
import { AgGridColumn, AgGridReact } from 'ag-grid-react';
import { GridOptions } from 'ag-grid-community';

import 'ag-grid-community/dist/styles/ag-grid.css';
import 'ag-grid-community/dist/styles/ag-theme-alpine.css';

function App() {
    const storageKey = "karaoke-storage";
    const [rowData, setRowData] = useState(getLocalStorageSongs());
    const [gridApi, setGridApi] = useState(null);
    const [gridColumnApi, setGridColumnApi] = useState(null);
    const searchDivStyle={backgroundColor:"#dedede",padding:10}
    const searchStyle={width:"100%",padding:"10px 20px",borderRadius:20,outline:0,
    border:"2px #68bf40 solid",fontSize:"100%"}
    
    const onGridReady = (params) => {
        setGridApi(params.api);
        setGridColumnApi(params.columnApi);
    };
    const onFilterTextChange=(e)=>{
      gridApi.setQuickFilter(e.target.value)
    }    
    useEffect(() => {
        var songs = getLocalStorageSongs();
        if (songs.length == 0) {
            fetchSongsRemotely();
            return;
        }
        setRowData(songs);
    }, []);
    function getLocalStorageSongs() {
        var songs = window.localStorage.getItem(storageKey);
        if (songs == null) {
            return [];
        }
        return JSON.parse(songs);
    }
    function fetchSongsRemotely() {
        fetch('http://karaoke-wedding.azurewebsites.net/songs')
            .then(result => {
                var test = result.json();
                return test;
            })
            .then(rows => {
                window.localStorage.setItem(storageKey, JSON.stringify(rows));
                setRowData(rows);
            });
    }

    return (
        <div>
            <h1 align="center">Karaoke night</h1>
            <div style={searchDivStyle}>
                <input type="search" style={searchStyle} onChange={onFilterTextChange} placeholder="search songs..." />
            </div>
            <div className="ag-theme-alpine" style={{ height: 600 }}>
                <AgGridReact rowData={rowData}
                    defaultColDef={{
                        flex: 1,
                        sortable: true,
                        resizable: true,
                        filter: true,
                    }}
                    onGridReady={onGridReady}>
                    <AgGridColumn field="name" sortable={true} filter={true} />
                    <AgGridColumn field="artist" sortable={true} filter={true} />
                </AgGridReact>
            </div>
        </div>
    );
};


export default App;