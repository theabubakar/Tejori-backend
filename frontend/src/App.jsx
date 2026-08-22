import { BrowserRouter, Navigate, Route, Routes } from 'react-router-dom';
import { AddBucketProvider } from './context/AddBucketContext';
import AccountCreatedPage from './pages/AccountCreatedPage';
import AddBucketAppointmentDetailsPage from './pages/add-bucket/AddBucketAppointmentDetailsPage';
import AddBucketContractDetailsPage from './pages/add-bucket/AddBucketContractDetailsPage';
import AddBucketContractDocumentsPage from './pages/add-bucket/AddBucketContractDocumentsPage';
import AddBucketCustomCategoriesPage from './pages/add-bucket/AddBucketCustomCategoriesPage';
import AddBucketCustomFormBuilderPage from './pages/add-bucket/AddBucketCustomFormBuilderPage';
import AddBucketCustomSubmitPage from './pages/add-bucket/AddBucketCustomSubmitPage';
import AddBucketMedicineDetailsPage from './pages/add-bucket/AddBucketMedicineDetailsPage';
import AddBucketStep1Page from './pages/add-bucket/AddBucketStep1Page';
import AddBucketSubmitPage from './pages/add-bucket/AddBucketSubmitPage';
import AddBucketTripDetailsPage from './pages/add-bucket/AddBucketTripDetailsPage';
import AddBucketWarrantyCategoryPage from './pages/add-bucket/AddBucketWarrantyCategoryPage';
import AddBucketWarrantyDetailsPage from './pages/add-bucket/AddBucketWarrantyDetailsPage';
import ForgotPasswordPage from './pages/ForgotPasswordPage';
import HomePage from './pages/HomePage';
import LoginPage from './pages/LoginPage';
import OtpVerificationPage from './pages/OtpVerificationPage';
import PasswordChangedPage from './pages/PasswordChangedPage';
import ProfilePage from './pages/ProfilePage';
import RegisterPage from './pages/RegisterPage';
import ResetPasswordPage from './pages/ResetPasswordPage';
import WelcomePage from './pages/WelcomePage';

export default function App() {
  return (
    <BrowserRouter>
      <AddBucketProvider>
        <Routes>
          <Route path="/" element={<WelcomePage />} />
          <Route path="/register" element={<RegisterPage />} />
          <Route path="/verify-otp" element={<OtpVerificationPage />} />
          <Route path="/login" element={<LoginPage />} />
          <Route path="/forgot-password" element={<ForgotPasswordPage />} />
          <Route path="/forgot-password/verify-otp" element={<OtpVerificationPage />} />
          <Route path="/reset-password" element={<ResetPasswordPage />} />
          <Route path="/account-created" element={<AccountCreatedPage />} />
          <Route path="/password-changed" element={<PasswordChangedPage />} />
          <Route path="/home" element={<HomePage />} />
          <Route path="/add-bucket" element={<AddBucketStep1Page />} />
          <Route path="/add-bucket/contract/details" element={<AddBucketContractDetailsPage />} />
          <Route path="/add-bucket/contract/documents" element={<AddBucketContractDocumentsPage />} />
          <Route path="/add-bucket/warranty/category" element={<AddBucketWarrantyCategoryPage />} />
          <Route path="/add-bucket/warranty/details" element={<AddBucketWarrantyDetailsPage />} />
          <Route path="/add-bucket/trip/details" element={<AddBucketTripDetailsPage />} />
          <Route path="/add-bucket/appointment/details" element={<AddBucketAppointmentDetailsPage />} />
          <Route path="/add-bucket/medicine/details" element={<AddBucketMedicineDetailsPage />} />
          <Route path="/add-bucket/custom/categories" element={<AddBucketCustomCategoriesPage />} />
          <Route path="/add-bucket/custom/form" element={<AddBucketCustomFormBuilderPage />} />
          <Route path="/add-bucket/custom/submit" element={<AddBucketCustomSubmitPage />} />
          <Route path="/add-bucket/submit" element={<AddBucketSubmitPage />} />
          <Route path="/profile" element={<ProfilePage />} />
          <Route path="/access" element={<Navigate to="/home" replace />} />
          <Route path="*" element={<Navigate to="/" replace />} />
        </Routes>
      </AddBucketProvider>
    </BrowserRouter>
  );
}
