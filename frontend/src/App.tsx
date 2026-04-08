import { BrowserRouter as Router, Routes, Route } from 'react-router-dom';
import { MainLayout } from './shared/layouts/MainLayout';
import { HomePage } from './features/home/pages/HomePage';
import { CalculateChartPage } from './features/chart/pages/CalculateChartPage';
import { SavedChartsPage } from './features/chart/pages/SavedChartsPage';
import { SavedChartDetailPage } from './features/chart/pages/SavedChartDetailPage';

function App() {
  return (
    <Router>
      <Routes>
        <Route path="/" element={<MainLayout />}>
          <Route index element={<HomePage />} />
          <Route path="chart/calculate" element={<CalculateChartPage />} />
          <Route path="charts/saved" element={<SavedChartsPage />} />
          <Route path="charts/saved/:chartId" element={<SavedChartDetailPage />} />
          {/* Default fallback route could be added here later */}
        </Route>
      </Routes>
    </Router>
  );
}

export default App;
