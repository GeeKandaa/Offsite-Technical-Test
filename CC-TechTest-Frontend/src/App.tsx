import { useEffect, useState } from 'react'
import './App.css'
import UploadFile from './components/UploadFile'
import DataDisplay from './components/DataDisplay'

function App() {
  const [isDarkMode, setIsDarkMode] = useState(false);
  const [activeTab, setActiveTab] = useState<'upload' | 'view'>('upload');

  useEffect(() => {
    const darkModePreferred = window.matchMedia('(prefers-color-scheme: dark)');
    setIsDarkMode(darkModePreferred.matches);

    const preferenceChangeHandler = (e: MediaQueryListEvent) => setIsDarkMode(e.matches);
    darkModePreferred.addEventListener('change', preferenceChangeHandler);
    return () => {
      darkModePreferred.removeEventListener('change', preferenceChangeHandler);
    };
  }, []);

  return (
    <>
      <div>
        <h1>CC Technical Test Application</h1>
        <div style={{ display: 'flex', gap: '1rem', marginBottom: '1rem' }}>
          <button
            onClick={() => setActiveTab('upload')}
            style={{
              padding: '0.5rem 1rem',
              borderBottom: activeTab === 'upload' ? '2px solid #0078d4' : '2px solid transparent',
              background: 'none',
              cursor: 'pointer',
              fontWeight: activeTab === 'upload' ? 'bold' : 'normal'
            }}
          >
            Upload File
          </button>
          <button
            onClick={() => setActiveTab('view')}
            style={{
              padding: '0.5rem 1rem',
              borderBottom: activeTab === 'view' ? '2px solid #0078d4' : '2px solid transparent',
              background: 'none',
              cursor: 'pointer',
              fontWeight: activeTab === 'view' ? 'bold' : 'normal'
            }}
          >
            View Uploaded Data
          </button>
        </div>
        {activeTab === 'upload' && (
          <>
            <h2>Upload a file</h2>
            <UploadFile DarkMode={isDarkMode} />
          </>
        )}
        {activeTab === 'view' && (
          <>
            <h2>View Uploaded Data</h2>
            <DataDisplay DarkMode={isDarkMode} />
          </>
        )}
      </div>
    </>
  )
}

export default App
